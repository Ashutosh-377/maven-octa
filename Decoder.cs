using System.ComponentModel;

public class Decoder
{
    public Instruction Decode(ushort instruction)
    {
        byte opcodeValue = (byte)((instruction >> 12) & 0b1111);
        byte destination = (byte)((instruction >> 8) & 0b1111);
        byte source = (byte)((instruction >> 4) & 0b1111);
        byte address = (byte)(instruction & 0xff);

        Opcode opcode = (Opcode)opcodeValue;

        switch (opcode)
        {
            case Opcode.LOADI:
                ValidateRegister(destination);
                break;

            case Opcode.ADD:
            case Opcode.SUB:
            case Opcode.AND:
            case Opcode.OR:
            case Opcode.XOR:
            case Opcode.CMP:
                ValidateRegister(destination);
                ValidateRegister(source);
                ValidateReservedBits((instruction & 0x000F) == 0, opcode.ToString());
                break;

            case Opcode.NOT:            
            case Opcode.INC:            
            case Opcode.DEC:
                ValidateRegister(destination);
                ValidateReservedBits((instruction & 0x00FF) == 0, opcode.ToString());
                break;

            case Opcode.LOAD:
            case Opcode.STORE:
                ValidateRegister(destination);
                break;

            case Opcode.JMP:
            case Opcode.JZ:
            case Opcode.JNZ:
                ValidateReservedBits((instruction & 0x0F00) == 0, opcode.ToString());
                break;

            case Opcode.EXT:
                return DecodeExtended(instruction, destination, source, address);
            
            default:
                throw new InvalidOperationException($"MAVEN-OCTA: Unknown opcode 0x{opcodeValue:X}.");
        }

        return new Instruction(opcode, destination, source, address);
    }

    private static Instruction DecodeExtended(ushort instruction, byte extendedValue, byte source, byte address)
    {
        if (extendedValue > (byte)ExtendedOpcode.IN)
        {
            throw new InvalidOperationException($"MAVEN-OCTA: Unknown extended opcode 0x{extendedValue:X}.");
        }

        ExtendedOpcode extendedOpcode = (ExtendedOpcode)extendedValue;

        switch (extendedOpcode)
        {
            case ExtendedOpcode.HALT:
                ValidateReservedBits((instruction & 0x0FFF) == 0, extendedOpcode.ToString());
                break;

            case ExtendedOpcode.RET:
                ValidateReservedBits((instruction & 0x00FF) == 0, extendedOpcode.ToString());
                break;

            case ExtendedOpcode.PUSH:
            case ExtendedOpcode.POP:
            case ExtendedOpcode.OUT:
            case ExtendedOpcode.IN:
                ValidateRegister(source);
                ValidateReservedBits((instruction & 0x000F) == 0, extendedOpcode.ToString());
                break;

            case ExtendedOpcode.CALL:
                break;

            case ExtendedOpcode.SVC:
            case ExtendedOpcode.IRET:
                ValidateReservedBits((instruction & 0x00FF) == 0, extendedOpcode.ToString());
                break;

            default:
                throw new InvalidOperationException($"MAVEN-OCTA: Unsupported extended opcode {extendedOpcode}.");
        }

        return new Instruction(Opcode.EXT, extendedValue, source, address, extendedOpcode);
    }

    private static void ValidateRegister(byte index)
    {
        if (index >= 8)
        {
            throw new InvalidOperationException($"MAVEN-OCTA: Invalid register R{index} valid registers are R0-R7.");
        }
    }

    private static void ValidateReservedBits(bool valid, string instruction)
    {
        if (!valid)
        {
            throw new InvalidOperationException($"MAVEN-OCTA: Invalid encoding for {instruction}.");
        }
    }
}