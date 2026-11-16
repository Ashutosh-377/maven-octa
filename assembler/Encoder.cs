using System;
using System.Reflection.Emit;

public class Encoder
{
    public ushort Encode(ParsedInstruction instruction)
    {
        switch (instruction.Opcode)
        {
            case "ADD":
            case "SUB":
            case "AND":
            case "OR":
            case "XOR":
            case "CMP":
                {
                    byte desination = ParseRegister(instruction.Operand1);
                    byte source = ParseRegister(instruction.Operand2);

                    ushort machineCode = 0;

                    Opcode opcode = Enum.Parse<Opcode>(instruction.Opcode, true);

                    machineCode |= (ushort)((byte)opcode << 12);
                    machineCode |= (ushort)(desination << 8);
                    machineCode |= (ushort)(source << 4);

                    return machineCode;
                }

            case "LOAD":
            case "STORE":
            case "JMP":
            case "JZ":
            case "JNZ":
                {
                    byte register = 0;

                    if (instruction.Opcode == "LOAD" || instruction.Opcode == "STORE")
                    {
                        register = ParseRegister(instruction.Operand1);
                    }

                    byte address = byte.Parse(instruction.Opcode == "LOAD" || instruction.Opcode == "STORE" ? instruction.Operand2 : instruction.Operand1);
                    Opcode opcode = Enum.Parse<Opcode>(instruction.Opcode, true);
                    ushort machineCode = 0;
                    machineCode |= (ushort)((byte)opcode << 12);
                    machineCode |= (ushort)(register << 8);
                    machineCode |= address;

                    return machineCode;
                }

            case "INC":
            case "DEC":
            case "NOT":
                {
                    byte destination = ParseRegister(instruction.Operand1);

                    Opcode opcode = Enum.Parse<Opcode>(instruction.Opcode, true);
                    ushort machineCode = 0;
                    machineCode |= (ushort)((byte)opcode << 12);
                    machineCode |= (ushort)(destination << 8);

                    return machineCode;
                }

            case "HALT":
            case "PUSH":
            case "POP":
            case "CALL":
            case "RET":
                {
                    ExtendedOpcode extendedOpcode = Enum.Parse<ExtendedOpcode>(instruction.Opcode, true);
                    ushort machineCode = 0;
                    machineCode |= (ushort)((byte)Opcode.EXT << 12);
                    machineCode |= (ushort)((byte)extendedOpcode << 8);

                    if (instruction.Opcode == "PUSH" || instruction.Opcode == "POP")
                    {
                        byte register = ParseRegister(instruction.Operand1);
                        machineCode |= (ushort)(register << 4);
                    }
                    else if (instruction.Opcode == "CALL")
                    {
                        byte address = byte.Parse(instruction.Operand1);
                        machineCode |= address;
                    }

                    return machineCode;
                }
        }

        if (instruction.Opcode == "LOADI")
        {
            byte destination = ParseRegister(instruction.Operand1);
            byte immediate = byte.Parse(instruction.Operand2);

            ushort machineCode = 0;

            machineCode |= (ushort)((byte)Opcode.LOADI << 12);
            machineCode |= (ushort)((byte)destination << 8);
            machineCode |= immediate;

            return machineCode;
        }

        throw new Exception($"Unsupported instruction: {instruction.Opcode}");
    }

    private byte ParseRegister(string text)
    {
        return byte.Parse(text.Substring(1));
    }
}