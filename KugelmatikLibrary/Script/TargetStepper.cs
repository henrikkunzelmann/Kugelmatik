using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary.Script
{
    public class TargetStepper : Target
    {
        public int StepperX { get; private set; }
        public int StepperY { get; private set; }

        public TargetStepper(int stepperX, int stepperY)
        {
            this.StepperX = stepperX;
            this.StepperY = stepperY;
        }

        public override IEnumerable<Stepper> EnumerateSteppers(Kugelmatik kugelmatik)
        {
            yield return kugelmatik.GetStepperByPosition(StepperX, StepperY);
        }
    }
}
