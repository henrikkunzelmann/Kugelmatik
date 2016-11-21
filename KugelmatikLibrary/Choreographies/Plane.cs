using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary.Choreographies
{
    public class Plane : IChoreographyFunction
    {
        public TimeSpan CycleTime { get; private set; }

        public Plane(TimeSpan cycleTime)
        {
            this.CycleTime = cycleTime;
        }

        public ushort GetHeight(Cluster cluster, TimeSpan time, int x, int y)
        {
            float cy = y / (float)cluster.Kugelmatik.StepperCountY;

            float maxSteps = cluster.Kugelmatik.ClusterConfig.MaxSteps;

            float dcy = cy - 0.5f;
            float range = Math.Abs(dcy) * maxSteps;
            float t = MathHelper.ConvertTime(time, CycleTime);

            double steps = 0.5f * maxSteps + range * t * Math.Sign(dcy);
            return (ushort)Math.Round(steps);
        }
    }
}
