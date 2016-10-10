using System.Collections.Generic;

namespace KugelmatikLibrary.Script
{
    public class TargetAll : Target
    {
        public override IEnumerable<Stepper> EnumerateSteppers(Kugelmatik kugelmatik)
        {
            return kugelmatik.EnumerateSteppers();
        }
    }
}
