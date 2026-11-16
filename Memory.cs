public class Memory
{
    private readonly byte[] data = new byte[MemoryMap.TotalMemory];

    public byte Read(byte address)
    {
        return data[address];
    }

    public void Write(byte address, byte value)
    {
        data[address] = value;
    }

    public ushort ReadWord(byte address)
    {
        ValidateWordAddress(address);
        byte high = data[address];
        byte low = data[address + 1];

        return (ushort)((high << 8) | low);
    }

    public void WriteWord(byte address, ushort value)
    {
        ValidateWordAddress(address);
        data[address] = (byte)(value >> 8);
        data[address + 1] = (byte)(value & 0xff);
    }

    public static void ValidateWordAddress(byte address)
    {
        if (address >= MemoryMap.TotalMemory - 1)
        {
            throw new ArgumentOutOfRangeException(nameof(address), address, "MAVEN-OCTA: Word access cannot cross memory boundary");
        }
    }
}