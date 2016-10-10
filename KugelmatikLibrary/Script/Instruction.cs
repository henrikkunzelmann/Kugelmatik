namespace KugelmatikLibrary.Script
{
    public abstract class Instruction
    {
        public int Timestamp { get; private set; }

        public Instruction(int timestamp)
        {
            this.Timestamp = timestamp;
        }

        public abstract void Execute(Kugelmatik kugelmatik);
    }
}
