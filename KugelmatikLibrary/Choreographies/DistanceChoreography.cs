using System;

namespace KugelmatikLibrary.Choreographies
{
    public class DistanceChoreography : IChoreographyFunction
    {
        public TimeSpan CycleTime { get; private set; }

        public DistanceChoreography(TimeSpan cycleTime)
        {
            this.CycleTime = cycleTime;
        }

        public ushort GetHeight(Cluster cluster, TimeSpan time, int x, int y)
        {
            float width = cluster.Kugelmatik.StepperCountX;
            float height = cluster.Kugelmatik.StepperCountY;

            float dt = (float)MathHelper.ConvertTime(time, CycleTime);
            float dist = MathHelper.Distance(x / width, y / height, dt, 0.5f);

            dist = MathHelper.Clamp(dist, 0, 1);
            return (ushort)Math.Round(dist * cluster.Kugelmatik.ClusterConfig.MaxSteps);
        }
    }
}
