using System;

namespace KugelmatikLibrary
{
    /// <summary>
    /// Stellt verschiedene mathematische Hilfsmethoden bereit.
    /// </summary>
    public static class MathHelper
    {
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

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

        /// <summary>
        /// Wandelt eine Zahl vom Interval [0, 1] in den Interval [-1, 1] um.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ConvertToOneToOne(double value)
        {
            return (value * 2) - 1;
        }

        /// <summary>
        /// Wandelt eine Zahl vom Interval [-1, 1] in den Interval [0, 1] um.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ConvertToZeroToOne(double value)
        {
            return (value + 1) * 0.5f;
        }

        public static float ConvertTime(TimeSpan time, TimeSpan cycle)
        {
            double timeMs = time.TotalMilliseconds;
            double cycleMs = cycle.TotalMilliseconds;

            double t = (timeMs % cycleMs) / cycleMs;
            // wrap
            if (t >= 0.5f)
                t = 1 - t;

            return (float)t;
        }
    }
}
