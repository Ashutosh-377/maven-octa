using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== MAVEN-OCTA SELF-PRINT TEST ===\n");

        CPU cpu = new CPU();

        // M = 77
        cpu.Memory.WriteWord(0x00, 0x004D);
        cpu.Memory.WriteWord(0x02, 0x0701);
        cpu.Memory.WriteWord(0x04, 0xF500);

        // A = 65
        cpu.Memory.WriteWord(0x06, 0x0041);
        cpu.Memory.WriteWord(0x08, 0x0701);
        cpu.Memory.WriteWord(0x0A, 0xF500);

        // V = 86
        cpu.Memory.WriteWord(0x0C, 0x0056);
        cpu.Memory.WriteWord(0x0E, 0x0701);
        cpu.Memory.WriteWord(0x10, 0xF500);

        // E = 69
        cpu.Memory.WriteWord(0x12, 0x0045);
        cpu.Memory.WriteWord(0x14, 0x0701);
        cpu.Memory.WriteWord(0x16, 0xF500);

        // N = 78
        cpu.Memory.WriteWord(0x18, 0x004E);
        cpu.Memory.WriteWord(0x1A, 0x0701);
        cpu.Memory.WriteWord(0x1C, 0xF500);

        // HALT
        cpu.Memory.WriteWord(0x1E, 0xF000);

        Console.WriteLine("=== CPU EXECUTION ===");

        while (!cpu.Halted)
        {
            cpu.Step();
        }

        Console.WriteLine("\n\n=== FINAL STATE ===");
        Console.WriteLine($"PC     = {cpu.PC:X2}");
        Console.WriteLine($"Mode   = {cpu.Mode}");
        Console.WriteLine($"Halted = {cpu.Halted}");
    }
}