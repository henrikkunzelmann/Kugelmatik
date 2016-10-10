namespace KugelmatikLibrary.Script
{
    public class InstructionMove : Instruction
    {
        public Target Target { get; private set; }
        public ushort Height { get; private set; }

        public InstructionMove(int timestamp, Target target, ushort height)
            : base(timestamp)
        {
            this.Target = target;
            this.Height = height;
        }

        public override void Execute(Kugelmatik kugelmatik)
        {
            foreach (Stepper stepper in Target.EnumerateSteppers(kugelmatik))
                stepper.Height = Height;
        }
    }
}
