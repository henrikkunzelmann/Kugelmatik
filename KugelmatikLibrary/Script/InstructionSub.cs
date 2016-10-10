namespace KugelmatikLibrary.Script
{
    public class InstructionSub : Instruction
    {
        public Target Target { get; private set; }
        public ushort Amount { get; private set; }

        public InstructionSub(int timestamp, Target target, ushort amount)
            : base(timestamp)
        {
            this.Target = target;
            this.Amount = amount;
        }

        public override void Execute(Kugelmatik kugelmatik)
        {
            foreach (Stepper stepper in Target.EnumerateSteppers(kugelmatik))
                stepper.Height -= Amount;
        }
    }
}
