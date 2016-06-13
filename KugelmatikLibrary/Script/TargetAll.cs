using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
