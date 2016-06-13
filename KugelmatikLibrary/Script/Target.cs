using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary.Script
{
    public abstract class Target
    {
        public abstract IEnumerable<Stepper> EnumerateSteppers(Kugelmatik kugelmatik);
    }
}
