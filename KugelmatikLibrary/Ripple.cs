using System;

namespace KugelmatikLibrary
{
    public class Ripple : IChoreography
    {
        public ushort GetHeight(Cluster cluster, TimeSpan time, int x, int y)
        {
            float width = cluster.Kugelmatik.StepperCountX;
            float height = cluster.Kugelmatik.StepperCountY;

            float dt = (float)time.TotalSeconds * 0.1f;

            double val = Math.Sin(0.3 * x + dt) * Math.Cos(0.3 * y + dt);
            val += 1;
            val /= 2;

            return (ushort)(val * cluster.Kugelmatik.ClusterConfig.MaxSteps);
        }
    }
}
