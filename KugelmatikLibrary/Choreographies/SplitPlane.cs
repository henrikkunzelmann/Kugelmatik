using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary.Choreographies
{
    public class SplitPlane : IChoreographyFunction
    {
        public TimeSpan CycleTime { get; private set; }
        public float Inclination { get; private set; }

        public SplitPlane(TimeSpan cycleTime, float inclination)
        {
            this.CycleTime = cycleTime;
            this.Inclination = inclination;
        }

        public ushort GetHeight(Cluster cluster, TimeSpan time, int x, int y)
        {
            float cx = x / (float)cluster.Kugelmatik.StepperCountX;
            float cy = y / (float)cluster.Kugelmatik.StepperCountY;

            float maxSteps = cluster.Kugelmatik.ClusterConfig.MaxSteps;

            float dcy = cy - 0.5f;
            float range = Math.Abs(dcy) * maxSteps;
            float t = MathHelper.ConvertTime(time, CycleTime);
            t = (float)MathHelper.ConvertToOneToOne(t);

            float dir = 1;
            if (cx < 0.1 || cx > 0.9 || cy < 0.1 || cy > 0.9)
                dir = -1;

            double steps = 0.5f * maxSteps + range * t * Math.Sign(dcy) * Inclination * dir;
            return (ushort)Math.Round(steps);
        }
    }
}
