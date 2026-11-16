public enum KernelService : byte
{
    IRET  = 0x0,
    KPRINT = 0x1,
    KREAD  = 0x2,
    KMEMR  = 0x3,
    KMEMW  = 0x4,
    KADD   = 0x5,
    KSUB   = 0x6,
    KAND  = 0x7,
    KOR   = 0x8,
    KXOR   = 0x9,
    KNOT   = 0xA,
    KINC   = 0xB,
    KDEC   = 0xC,
    KCMP   = 0xD
}