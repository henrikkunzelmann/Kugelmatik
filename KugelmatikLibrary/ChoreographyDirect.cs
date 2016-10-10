using System;

namespace KugelmatikLibrary
{
    public class ChoreographyDirect : Choreography
    {
        public IChoreography Function { get; private set; }

        public ChoreographyDirect(IChoreography function)
        {
            this.Function = function;
        }

        public override void Tick(Kugelmatik kugelmatik, TimeSpan time)
        {
            for (int x = 0; x < kugelmatik.StepperCountX; x++)
            {
                for (int y = 0; y < kugelmatik.StepperCountY; y++)
                {
                    Stepper stepper = kugelmatik.GetStepperByPosition(x, y);
                    stepper.Set(Function.GetHeight(stepper.Cluster, time, x, y));
                }
            }
        }
    }
}
