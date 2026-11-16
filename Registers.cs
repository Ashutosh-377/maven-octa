public class Registers
{
    private readonly byte[] registers = new byte[8];

    public byte this[int index]
    {
        get => registers[index];
        set => registers[index] = value;
    }

    public int count => registers.Length;
}