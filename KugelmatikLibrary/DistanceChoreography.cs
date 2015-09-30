using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary
{
    public class DistanceChoreography : IChoreography
    {
        public ushort GetHeight(Config config, TimeSpan time, int x, int y)
        {
            float width = config.KugelmatikWidth * Cluster.Width;
            float height = config.KugelmatikHeight * Cluster.Height;

            float dt = (float)time.TotalMilliseconds * 0.05f;

            double dist = MathHelper.Distance(x / width, y / height, (dt % 1000) / 1000f, 0.5f);
            if (dist < 0)
                dist = 0;
            if (dist > 1)
                dist = 1;
            return (ushort)(dist * config.MaxHeight);
        }
    }
}
