using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary.Choreographies
{
    public class LinearMove : IChoreographyFunction
    {
        public TimeSpan CycleTime { get; private set; }

        public LinearMove(TimeSpan cycleTime)
        {
            this.CycleTime = cycleTime;
        }

        public ushort GetHeight(Cluster cluster, TimeSpan time, int x, int y)
        {
            double cycle = CycleTime.TotalMilliseconds;
            double t = time.TotalMilliseconds % cycle;

            if (t > cycle * 0.5)
                return 0;
            return (ushort)cluster.Kugelmatik.ClusterConfig.MaxSteps;
        }
    }
}
