namespace KugelmatikLibrary.Protocol
{
    public enum PacketType : byte
    {
        Ping = 1,
        Ack = 2,

        Stepper = 3,
        Steppers = 4,
        SteppersArray = 5,
        SteppersRectangle = 6,
        SteppersRectangleArray = 7,
        AllSteppers = 8,
        AllSteppersArray = 9,

        Home = 10,

        ResetRevision = 11,
        Fix = 12,

        HomeStepper = 13,

        GetData = 14,
        Info = 15,
        Config = 16,

        BlinkGreen = 17,
        BlinkRed = 18,

        Stop = 19,

        SetData = 20
    }
}
