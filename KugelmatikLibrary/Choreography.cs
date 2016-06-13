using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary
{
    public abstract class Choreography
    {
        public abstract void Tick(Kugelmatik kugelmatik, TimeSpan time);
    }
}
