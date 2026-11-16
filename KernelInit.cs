public static class KernelInit
{
    public static void Initialize(Memory memory)
    {
        memory.WriteWord(MemoryMap.KernelStart, 0xF600);
        memory.WriteWord(0xC2, 0xF700);
        memory.WriteWord(0xC4, 0xF800);
        memory.WriteWord(0xC6, 0x30DC);
        memory.WriteWord(0xC8, 0x20DE);
        memory.WriteWord(0xCA, 0x1010);
        memory.WriteWord(0xCC, 0x4010);
        memory.WriteWord(0xCE, 0x5010);
        memory.WriteWord(0xD0, 0x6010);
        memory.WriteWord(0xD2, 0x7010);
        memory.WriteWord(0xD4, 0x8000);
        memory.WriteWord(0xD6, 0x9000);
        memory.WriteWord(0xD8, 0xA000);
        memory.WriteWord(0xDA, 0xB010);
        memory.Write(0xDC, 0);
        memory.Write(0xDD, 0);
        memory.Write(0xDE, 0);
        memory.Write(0xDF, 0);
    }
}