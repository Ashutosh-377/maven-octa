public class ALU
{
    public Flags Flags { get; private set; } = Flags.None;

    public byte Add(byte a, byte b)
    {
        ushort result = (ushort)(a + b);
        byte value = (byte)result;

        SetZeroFlag(value);
        SetNegativeFlag(value);

        if (result > 0xff)
        {
            Flags |= Flags.Carry;
        }
        else
        {
            Flags &= ~Flags.Carry;
        }

        bool overflow = (~(a ^ b) & (a ^ value) & 0x80) != 0;

        if (overflow)
        {
            Flags |= Flags.Overflow;
        }
        else
        {
            Flags &= ~Flags.Overflow;
        }

        return value;
    }

    public byte Sub(byte a, byte b)
    {
        int result = a - b;
        byte value = (byte)result;

        SetZeroFlag(value);
        SetNegativeFlag(value);

        if (a < b)
        {
            Flags |= Flags.Carry;
        }
        else
        {
            Flags &= ~Flags.Carry;
        }

        bool overflow = ((a ^ b) & (a ^ value) & 0x80) != 0;

        if (overflow)
        {
            Flags |= Flags.Overflow;
        }
        else
        {
            Flags &= ~Flags.Overflow;
        }

        return value;
    }

    public byte And(byte a, byte b)
    {
        byte result = (byte)(a & b);
        SetLogicalFlags(result);
        return result;
    }

    public byte Or(byte a, byte b)
    {
        byte result = (byte)(a | b);
        SetLogicalFlags(result);
        return result;
    }

    public byte Xor(byte a, byte b)
    {
        byte result = (byte)(a ^ b);
        SetLogicalFlags(result);
        return result;
    }

    public byte Not(byte a)
    {
        byte result = (byte)~a;
        SetLogicalFlags(result);
        return result;
    }

    public byte Inc(byte value)
    {
        return Add(value, 1);
    }

    public byte Dec(byte value)
    {
        return Sub(value, 1);
    }

    public void Compare(byte a, byte b)
    {
        Sub(a, b);
    }

    private void SetZeroFlag(byte result)
    {
        if(result == 0)
        {
            Flags |= Flags.Zero;
        }
        else
        {
            Flags &= ~Flags.Zero;
        }
    }

    private void SetNegativeFlag(byte result)
    {
        if ((result & 0x80) != 0)
        {
            Flags |= Flags.Negative;
        }
        else
        {
            Flags &= ~Flags.Negative;
        }
    }

    private void SetLogicalFlags(byte result)
    {
        SetZeroFlag(result);
        SetNegativeFlag(result);

        Flags &= ~Flags.Carry;
        Flags &= ~Flags.Overflow;
    }
}