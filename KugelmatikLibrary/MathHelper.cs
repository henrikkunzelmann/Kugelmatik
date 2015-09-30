using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary
{
    public static class MathHelper
    {
        /// <summary>
        /// Gibt die Entfernung zwischen zwei Punkten (x1, y1) und (x2, y2) zurück. 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns>Die Entfernung.</returns>
        public static float Distance(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt(DistanceSquared(x1, y1, x2, y2));
        }

        /// <summary>
        /// Gibt die Entfernung zwischen zwei Punkten (x1, y1) und (x2, y2) zum Quadrat zurück.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns>Die Entfernung zum Quadrat.</returns>
        public static float DistanceSquared(float x1, float y1, float x2, float y2)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            return dx * dx + dy * dy;
        }
    }
}
