public class ParsedInstruction
{
    public string Opcode { get; }
    public string Operand1 { get; }
    public string Operand2 { get; }

    public ParsedInstruction(string opcode, string operand1, string operand2)
    {
        Opcode = opcode;
        Operand1 = operand1;
        Operand2 = operand2;
    }

    public override string ToString()
    {
        return $"{Opcode} | {Operand1} | {Operand2}";
    }
}