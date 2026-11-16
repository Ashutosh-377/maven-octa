using System;
using System.Collections.Generic;

public class Assembler
{
    private readonly Lexer lexer = new Lexer();
    private readonly Parser parser = new Parser();
    private readonly Encoder encoder = new Encoder();
    private readonly LabelPass labelPass = new LabelPass();

    public List<ushort> Assemble(string source)
    {
        LabelTable labels = labelPass.DiscoverLabels(source);

        List<ushort> machineCode = new List<ushort>();

        string[] lines = source.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();
            if (line.Length == 0 || line.EndsWith(":"))
            {
                continue;
            }

            line = ResolveLabelOperand(line, labels);

            var tokens = lexer.Tokenize(line);
            var parsed = parser.Parse(tokens);
            ushort instruction = encoder.Encode(parsed);
            machineCode.Add(instruction);
        }

        MemoryMap.ValidateProgramRange(MemoryMap.ProgramStart, machineCode.Count * 2);

        return machineCode;
    }

    private string ResolveLabelOperand(string line, LabelTable labels)
    {
        string[] parts = line.Split(new[] {' ', '\t', ','}, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
        {
            return line;
        }

        string opcode = parts[0].ToUpperInvariant();
        if(opcode != "JMP" && opcode != "JZ" && opcode != "JNZ" && opcode != "CALL")
        {
            return line;
        }

        string target = parts[1];
        if (byte.TryParse(target, out _))
        {
            return line;
        }

        byte address = labels.GetAddress(target);

        return $"{parts[0]} {address}";
    }
}
