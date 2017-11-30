namespace KugelmatikLibrary
{
    public enum ErrorCode
    {
        None = 0,
        PacketTooShort = 1,
        InvalidX = 2,
        InvalidY = 3,
        InvalidMagic = 4,
        BufferOverflow = 5,
        UnknownPacket = 6,
        NotRunningBusy = 7,
        InvalidConfigValue = 8,
        InvalidHeight = 9,
        InvalidValue = 10,
        NotAllowedToRead = 11,
        PacketSizeBufferOverflow = 12,

        McpFault1 = 13,
        McpFault2 = 14,
        McpFault3 = 15,
        McpFault4 = 16,
        McpFault5 = 17,
        McpFault6 = 18,
        McpFault7 = 19,
        McpFault8 = 20,
        OTAFailed = 21,

        InternalWrongParameter = 251,
        InternalWrongLoopValues = 252,
        InternalInvalidTimerIndex = 253,
        InternalDefaultConfigFault = 254,
        Internal = 255,
        UnknownError
    }
}
