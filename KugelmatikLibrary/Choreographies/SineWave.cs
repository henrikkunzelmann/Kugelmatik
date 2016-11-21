using System;

namespace KugelmatikLibrary.Choreographies
{
    /// <summary>
    /// Stellt eine Sinus-Welle als Choreographie dar. 
    /// </summary>
    public class SineWave : IChoreographyFunction
    {
        public Direction WaveDirection { get; private set; }
        public float Frequency { get; private set; }

        public SineWave(Direction waveDirection, float frequency)
        {
            if (frequency == 0)
                throw new ArgumentOutOfRangeException(nameof(frequency));

            this.WaveDirection = waveDirection;
            this.Frequency = frequency;
        }

        public ushort GetHeight(Cluster cluster, TimeSpan time, int x, int y)
        {
            float v = x / (float)cluster.Kugelmatik.StepperCountX;
            if (WaveDirection == Direction.Y)
                v = y / (float)cluster.Kugelmatik.StepperCountY;

            v *= (float)Math.PI * 2;

            // Sinuswelle erstellen
            double sinWave = Math.Sin(v + time.TotalSeconds * Frequency);
            sinWave = MathHelper.ConvertToZeroToOne(sinWave);

            // in Schritte umwandeln
            return (ushort)Math.Round(sinWave * cluster.Kugelmatik.ClusterConfig.MaxSteps);
        }
    }
}
