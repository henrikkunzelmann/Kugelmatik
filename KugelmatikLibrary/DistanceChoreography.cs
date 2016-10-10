using System;

namespace KugelmatikLibrary
{
    public class DistanceChoreography : IChoreography
    {
        public ushort GetHeight(Cluster cluster, TimeSpan time, int x, int y)
        {
            float width = cluster.Kugelmatik.StepperCountX;
            float height = cluster.Kugelmatik.StepperCountY;

            float dt = (float)time.TotalMilliseconds * 0.05f;

            double dist = MathHelper.Distance(x / width, y / height, (dt % 1000) / 1000f, 0.5f);
            if (dist < 0)
                dist = 0;
            if (dist > 1)
                dist = 1;
            return (ushort)(dist * cluster.Kugelmatik.ClusterConfig.MaxSteps);
        }
    }
}
