using System.Reflection.Emit;

public class CPU
{
    public Registers Registers { get; } = new Registers();
    public byte PC { get; private set; }
    public Flags Flags { get; private set; } = Flags.None;
    public bool Halted { get; private set; }
    public Memory Memory { get; }
    private readonly Decoder decoder = new Decoder();
    private readonly ALU alu = new ALU();
    public byte SP { get; private set; }
    private int stackDepth = 0;
    private const int StackCapacity = MemoryMap.StackEnd - MemoryMap.StackStart + 1;
    public ExecutionMode Mode { get; private set; } = ExecutionMode.User;
    private byte savedUserPC;
    private bool kernelTransitionActive;

    public CPU() : this(0x00, ExecutionMode.User) {}

    internal CPU(byte initialPC, ExecutionMode initialMode)
    {
        Memory = new Memory();
        KernelInit.Initialize(Memory);
        SP = MemoryMap.InitialStackPointer;
        PC = initialPC;
        Mode = initialMode;
    }

    public void Step()
    {

        if (Mode == ExecutionMode.User)
        {
            MemoryMap.ValidateInstructionAddress(PC, "USER interaction fetch");
        }
        else
        {
            MemoryMap.ValidateKernelInstructionAddress(PC, "Kernel instruction fetched");
        }

        ExecutionMode previousMode = Mode;

        ushort rawInstruction = Memory.ReadWord(PC);
        Instruction instruction = decoder.Decode(rawInstruction);

        bool pcChanged = Execute(instruction);
        if (Halted)
        {
            return;
        }

        if (previousMode == ExecutionMode.Kernel && Mode == ExecutionMode.Kernel)
        {
            PC = MemoryMap.KernelStart;
            return;
        }

        if(!pcChanged)
        {
            PC += 2;
        }
    }

    private bool Execute(Instruction instruction)
    {
        switch (instruction.Opcode)
        {
            case Opcode.LOADI:
                Registers[instruction.Destination] = instruction.Address;
                return false;
            
            case Opcode.ADD:
                Registers[instruction.Destination] = alu.Add(Registers[instruction.Destination], Registers[instruction.Source]);
                return false;

            case Opcode.SUB:
                Registers[instruction.Destination] = alu.Sub(Registers[instruction.Destination], Registers[instruction.Source]);
                Flags = alu.Flags;
                return false;

            case Opcode.AND:
                Registers[instruction.Destination] = alu.And(Registers[instruction.Destination], Registers[instruction.Source]);
                Flags = alu.Flags;
                return false;

            case Opcode.OR:
                Registers[instruction.Destination] = alu.Or(Registers[instruction.Destination], Registers[instruction.Source]);
                Flags = alu.Flags;
                return false;

            case Opcode.XOR:
                Registers[instruction.Destination] = alu.Xor(Registers[instruction.Destination], Registers[instruction.Source]);
                Flags = alu.Flags;
                return false;

            case Opcode.NOT:
                Registers[instruction.Destination] = alu.Not(Registers[instruction.Destination]);
                Flags = alu.Flags;
                return false;

            case Opcode.INC:
                Registers[instruction.Destination] = alu.Inc(Registers[instruction.Destination]);
                Flags = alu.Flags;
                return false;

            case Opcode.DEC:
                Registers[instruction.Destination] = alu.Dec(Registers[instruction.Destination]);
                Flags = alu.Flags;
                return false;

            case Opcode.CMP:
                alu.Compare(Registers[instruction.Destination], Registers[instruction.Source]);
                Flags = alu.Flags;
                return false;

            case Opcode.LOAD:
                ValidateProgramDataAddress(instruction.Address, "LOAD");
                Registers[instruction.Destination] = Memory.Read(instruction.Address);
                return false;

            case Opcode.STORE:
                ValidateProgramDataAddress(instruction.Address, "STORE");
                Memory.Write(instruction.Address, Registers[instruction.Destination]);
                return false;

            case Opcode.JMP:
                if (Mode == ExecutionMode.User)
                {
                    MemoryMap.ValidateInstructionAddress(instruction.Address, "JMP");
                }
                else
                {
                    MemoryMap.ValidateKernelInstructionAddress(instruction.Address, "JMP");
                }
                
                PC = instruction.Address;
                return true;

            case Opcode.JZ:
                if ((Flags & Flags.Zero) != 0)
                {
                    MemoryMap.ValidateInstructionAddress(instruction.Address, "JZ");
                    PC = instruction.Address;
                    return true;
                }
                return false;

            case Opcode.JNZ:
                if ((Flags & Flags.Zero) == 0)
                {
                    MemoryMap.ValidateInstructionAddress(instruction.Address, "JNZ");
                    PC = instruction.Address;
                    return true;
                }
                return false;

            case Opcode.EXT:
                return ExecuteExtended(instruction);

            default:
                throw new Exception($"MAVEN-OCTA: Unsupported opcode {instruction.Opcode}.");
        }
    }

    private bool ExecuteExtended(Instruction instruction)
    {
        switch (instruction.ExtendedOpcode)
        {
            case ExtendedOpcode.HALT:
                Halted = true;
                return false;
                
            case ExtendedOpcode.PUSH:
            {
                ValidateStackState(requireItem: false);

                if (stackDepth >= StackCapacity)
                {
                    throw new InvalidOperationException("MAVEN-OCTA: Stack overflow on PUSH.");
                }

                byte value = Registers[instruction.Source];
                Memory.Write(SP, value);
                SP--;
                stackDepth++;
                return false;
            }

            case ExtendedOpcode.POP:
            {
                ValidateStackState(requireItem: true);

                int nextAddress = SP + 1;
                ValidateStackAddress(nextAddress);
                
                byte value = Memory.Read((byte)nextAddress);
                Registers[instruction.Source] = value;
                SP = (byte)nextAddress;
                stackDepth--;
                return false;
            }

            case ExtendedOpcode.CALL:
            {
                MemoryMap.ValidateInstructionAddress(instruction.Address, "CALL target");
                int retrunAddress = PC + 2;
                MemoryMap.ValidateInstructionAddress(retrunAddress, "CALL return");
                ValidateStackState(requireItem: false);

                if (stackDepth >= StackCapacity)
                {
                    throw new InvalidOperationException("MAVEN-OCTA: Stack overflow on CALL.");
                }

                Memory.Write(SP, (byte)retrunAddress);
                SP = (byte)(SP - 1);
                stackDepth++;
                PC = instruction.Address;
                return true;
            }

            case ExtendedOpcode.RET:
            {
                ValidateStackState(requireItem: true);

                int nextAddress = SP + 1;
                ValidateStackAddress(nextAddress);

                byte returnAddress = Memory.Read((byte)nextAddress);
                MemoryMap.ValidateInstructionAddress(returnAddress, "RET target");

                SP = (byte)nextAddress;
                stackDepth--;
                PC = returnAddress;
                return true;
            }

            case ExtendedOpcode.SVC:
            {
                if (Mode != ExecutionMode.User)
                {
                    throw new InvalidOperationException("MAVEN-OCTA: SVC is only allowed in user mode.");
                }        
                if (kernelTransitionActive)
                {
                    throw new InvalidOperationException("MAVEN-OCTA: A kernel transition is already active.");
                }

                byte serviceNumber = Registers[7];
                if (serviceNumber > 13)
                {
                    throw new InvalidOperationException($"MAVEN-OCTA: Invalid system call number {serviceNumber}.");
                }

                int returnAddress = PC + 2;
                MemoryMap.ValidateInstructionAddress(returnAddress, "SVC return");

                int kernelEntryAddress = MemoryMap.KernelStart + (serviceNumber * 2);

                MemoryMap.ValidateKernelInstructionAddress(kernelEntryAddress, "SVC entry");

                savedUserPC = (byte)returnAddress;
                kernelTransitionActive = true;

                Mode = ExecutionMode.Kernel;
                PC = (byte)kernelEntryAddress;

                return true;
            }

            case ExtendedOpcode.IRET:
            {
                if (Mode != ExecutionMode.Kernel)
                {
                    throw new InvalidOperationException("MAVEN-OCTA: IRET is only allowed in kernel mode.");
                }        
                if (!kernelTransitionActive)
                {
                    throw new InvalidOperationException("MAVEN-OCTA: IRET has no active kernel transition");
                }

                MemoryMap.ValidateInstructionAddress(savedUserPC, "IRET return");
                PC = savedUserPC;
                Mode = ExecutionMode.User;
                kernelTransitionActive = false;

                return true;
            }

            case ExtendedOpcode.OUT:
            {
                byte value = Registers[instruction.Source];
                Console.Write((char)value);
                return false;       
            }

            case ExtendedOpcode.IN:
            {
                byte value = (byte)Console.ReadKey(intercept: true).KeyChar;
                Registers[instruction.Source] = value;
                return false;            
            }

            default:
                throw new Exception($"MAVEN-OCTA: Unsupported extended opcode {instruction.ExtendedOpcode}.");
        }
    }

    private void ValidateProgramDataAddress(byte address, string operation)
    {
        if (Mode == ExecutionMode.User)
        {
            if (address < MemoryMap.ProgramStart || address > MemoryMap.ProgramEnd)
            {
                throw new InvalidOperationException($"MAVEN-OCTA: {operation} address 0x{address:X2} is outside program memory.");
            }
        }
        else
        {
            bool validProgramAddress = address >= MemoryMap.ProgramStart && address <= MemoryMap.ProgramEnd;
            bool validKernelDataAddress = MemoryMap.IsValidKernelDataAddress(address);

            if (!validProgramAddress && !validKernelDataAddress)
            {
                throw new InvalidOperationException($"MAVEN-OCTA: {operation} address 0x{address:X2} is not accessible to the kernel.");
            }
        }
    }

    private static void ValidateStackAddress(int address)
    {
        if (address < MemoryMap.StackStart || address > MemoryMap.StackEnd)
        {
            throw new InvalidOperationException($"MAVEN-OCTA: Stack address 0x{address:X} is outside usable stack memory.");
        }
    }

    private void ValidateStackState(bool requireItem)
    {
        if (stackDepth < 0 || stackDepth > StackCapacity)
        {
            throw new InvalidOperationException("MAVEN-OCTA: Stack depth is inconsistent.");
        }

        int expectedSP = MemoryMap.StackEnd - stackDepth;

        if (SP != expectedSP)
        {
            throw new InvalidOperationException("MAVEN-OCTA: Stack pointer and stack depth disagree.");
        }

        if (requireItem && stackDepth == 0)
        {
            throw new InvalidOperationException("MAVEN-OCTA: Stack underflow.");
        }
    }
}