using System;

namespace KugelmatikLibrary
{
    /// <summary>
    /// Stellt eine Sinus-Welle als Choreographie dar. 
    /// </summary>
    public class SineWave : IChoreography
    {
        public Direction WaveDirection { get; private set; }

        public float TimeFactor { get; private set; }
        public float Frequency { get; private set; }

        public SineWave(Direction waveDirection, float timeFactor, float frequency)
        {
            if (frequency == 0)
                throw new ArgumentOutOfRangeException("frequency");

            this.WaveDirection = waveDirection;
            this.TimeFactor = timeFactor;
            this.Frequency = frequency;
        }

        public ushort GetHeight(Cluster cluster, TimeSpan time, int x, int y)
        {
            float v = x;
            if (WaveDirection == Direction.Y)
                v = y;

            // Sinuswelle erstellen
            double sinWave = Math.Sin((v + time.TotalMilliseconds * TimeFactor) * Frequency);

            sinWave += 1; // in den Bereich [0, 2] verschieben
            sinWave /= 2; // normalisieren

            // in Schritte umwandeln
            return (ushort)(sinWave * cluster.Kugelmatik.ClusterConfig.MaxSteps);
        }

        public enum Direction
        {
            X, Y
        }
    }
}
