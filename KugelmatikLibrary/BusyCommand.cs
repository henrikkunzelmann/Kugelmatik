namespace KugelmatikLibrary
{
    public enum BusyCommand : byte
    {
        None = 0,
        Home = 1,
        Fix = 2,
        HomeStepper = 3,
        Unknown = 255
    }
}
