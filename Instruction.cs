public struct Instruction
{
    public Opcode Opcode { get; }
    public byte Destination { get; }
    public byte Source { get; }
    public byte Address { get; }
    public ExtendedOpcode ExtendedOpcode { get; }

    public Instruction(Opcode opcode, byte destination, byte source, byte address, ExtendedOpcode extendedOpcode = 0)
    {
        Opcode = opcode;
        Destination = destination;
        Source = source;
        Address = address;
        ExtendedOpcode = extendedOpcode;
    }
}