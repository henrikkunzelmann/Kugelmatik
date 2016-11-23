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
            float cy = y / (float)cluster.Kugelmatik.StepperCountY;

            float maxSteps = cluster.Kugelmatik.ClusterConfig.MaxSteps;

            float dcy = cy - 0.5f;
            float range = Math.Abs(dcy) * maxSteps;
            float t = MathHelper.ConvertTime(time, CycleTime);
            t = (float)MathHelper.ConvertToOneToOne(t);

            float dir = 1;

            int borderX1 = cluster.Kugelmatik.Config.KugelmatikWidth;
            int borderY1 = cluster.Kugelmatik.Config.KugelmatikHeight;
            int borderX2 = cluster.Kugelmatik.StepperCountX - borderX1;
            int borderY2 = cluster.Kugelmatik.StepperCountY - borderY1;

            if (x < borderX1 || x >= borderX2 || y < borderY1 || y >= borderY2)
                dir = -1;

            double steps = 0.5f * maxSteps + range * t * Math.Sign(dcy) * Inclination * dir;
            return (ushort)Math.Round(steps);
        }
    }
}
