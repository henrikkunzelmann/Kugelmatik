using System.Collections.Generic;

namespace KugelmatikLibrary.Script
{
    public abstract class Target
    {
        public abstract IEnumerable<Stepper> EnumerateSteppers(Kugelmatik kugelmatik);
    }
}
