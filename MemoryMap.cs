using System;

public static class MemoryMap
{
    public const int TotalMemory = 256;

    public const byte ProgramStart = 0x00;
    public const byte ProgramEnd = 0xBF;
    public const byte KernelStart = 0xC0;
    public const byte KernelEnd = 0xDF;
    public const byte KernelInsructionStart = 0xC0;
    public const byte KernelInsructionEnd = 0xDB;
    public const byte KernelDataStart = 0xDC;
    public const byte KernelDataEnd = 0xDF;
    public const int KernelDataSize = 4;
    public const byte StackStart = 0xE0;
    public const byte StackEnd = 0xFF;

    public const byte InitialStackPointer = StackEnd;

    public static bool IsValidProgramRange(int start, int byteLength)
    {
        if (byteLength < 0)
        {
            return false;
        }
        if (start < ProgramStart || start > ProgramEnd + 1)
        {
            return false;
        }
        if (byteLength == 0)
        {
            return true;
        }
        return start + byteLength - 1 <= ProgramEnd;
    }

    public static void ValidateProgramRange(int start, int byteLength)
    {
        if (!IsValidProgramRange(start, byteLength))
        {
            throw new InvalidOperationException($"MAVEN-OCTA: Program range 0x{start:X} (length {byteLength} bytes) exceeds program memory 0x{ProgramStart:X2}-0x{ProgramEnd:X2}.");
        }
    }

    public static bool IsValidProgramInstructionAddress(int address)
    {
        return address >= ProgramStart && address + 1 <= ProgramEnd && address % 2 == 0;
    }

    public static bool IsValidKernelAddress(int address)
    {
        return address >= KernelStart && address + 1 <= KernelEnd && address % 2 == 0;
    }

    public static bool IsValidKernelInstructionAddress(int address)
    {
        return address >= KernelInsructionStart && address + 1 <= KernelInsructionEnd && address % 2 == 0;
    }

    public static bool IsValidKernelDataAddress(int address)
    {
        return address >= KernelDataStart && address <= KernelDataEnd;
    }

    public static bool IsValidStackAddress(int address)
    {
        return address >= StackStart && address + 1 <= StackEnd;
    }

    public static void ValidateInstructionAddress(int address, string operation)
    {
        if (!IsValidProgramInstructionAddress(address))
        {
            throw new InvalidOperationException($"MAVEN-OCTA: {operation} address 0x{address:X} is not a valid program instruction address.");
        }
    }

    public static void ValidateKernelAddress(int address, string operation)
    {
        if (!IsValidKernelAddress(address))
        {
            throw new InvalidOperationException($"MAVEN-OCTA: {operation} address 0x{address:X} is not a valid kernel address.");
        }
    }

    public static void ValidateKernelInstructionAddress(int address, string operation)
    {
        if (!IsValidKernelInstructionAddress(address))
        {
            throw new InvalidOperationException($"MAVEN-OCTA: {operation} address 0x{address:X} is not a valid kernel instruction address.");
        }
    }

    public static void ValidateKernelDataAddress(int address, string operation)
    {
        if (!IsValidKernelDataAddress(address))
        {
            throw new InvalidOperationException($"MAVEN-OCTA: {operation} address 0x{address:X2} is not a valid kernel data address.");
        }
    }
}