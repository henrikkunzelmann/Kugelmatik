using System;

namespace KugelmatikLibrary
{
    public abstract class Choreography
    {
        public abstract void Tick(Kugelmatik kugelmatik, TimeSpan time);
    }
}
