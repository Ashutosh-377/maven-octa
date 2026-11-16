public enum ExtendedOpcode : byte
{
    HALT = 0x0,
    PUSH = 0x1,
    POP = 0x2,
    CALL = 0x3,
    RET = 0x4,
    SVC = 0x5,
    IRET = 0x6,
    OUT = 0x07,
    IN = 0x08
}