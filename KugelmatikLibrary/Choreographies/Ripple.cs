using System;

namespace KugelmatikLibrary.Choreographies
{
    public class Ripple : IChoreographyFunction
    {
        public ushort GetHeight(Cluster cluster, TimeSpan time, int x, int y)
        {
            float width = cluster.Kugelmatik.StepperCountX;
            float height = cluster.Kugelmatik.StepperCountY;

            float dt = (float)time.TotalSeconds * 0.05f;

            float vx = (x / width) * 16;
            float vy = (y / height) * 16;

            double val = Math.Sin(0.3 * vx + dt) * Math.Cos(0.3 * vy + dt);
            val = MathHelper.ConvertToZeroToOne(val);

            return (ushort)(val * cluster.Kugelmatik.ClusterConfig.MaxSteps);
        }
    }
}
