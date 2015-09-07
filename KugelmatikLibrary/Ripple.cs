using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary
{
    public class Ripple : IChoreography
    {
        public ushort GetHeight(Config config, TimeSpan time, int x, int y)
        {
            float width = config.KugelmatikWidth * Cluster.Width;
            float height = config.KugelmatikHeight * Cluster.Height;

            float dt = (float)time.TotalSeconds * 0.1f;

            double val = Math.Sin(0.3 * x + dt) * Math.Cos(0.3 * y + dt);
            val += 1;
            val /= 2;

            return (ushort)(val * config.MaxHeight);
        }
    }
}
