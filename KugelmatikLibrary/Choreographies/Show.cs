using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary.Choreographies
{
    public class Show : Choreography
    {
        public ShowEntry[] Entries { get; private set; }
        public TimeSpan InterpolationTime { get; private set; }

        private int current = 0;
        private TimeSpan timeSum = TimeSpan.Zero;

        public Show(ShowEntry[] entries, TimeSpan interpolationTime)
        {
            this.Entries = entries;
            this.InterpolationTime = interpolationTime;
        }

        public override void Tick(Kugelmatik kugelmatik, TimeSpan time)
        {
            if (current >= Entries.Length)
                current = 0;

            ShowEntry entry = Entries[current];

            TimeSpan endTime = timeSum + entry.Time; // Zeitpunkt wenn der Eintrag vorbei ist
            TimeSpan interpolationStart = endTime - InterpolationTime;

            // nächsten Eintrag
            if (time >= endTime)
            {
                timeSum += endTime;
                current++;
            }
            else if (time >= interpolationStart)
            {
                ShowEntry next;
                if (current + 1 >= Entries.Length)
                    next = Entries[0];
                else
                    next = Entries[current + 1];

                float pos = (float)((time - interpolationStart).TotalMilliseconds / InterpolationTime.TotalMilliseconds);

                for (int x = 0; x < kugelmatik.StepperCountX; x++)
                    for (int y = 0; y < kugelmatik.StepperCountY; y++)
                    {
                        Stepper stepper = kugelmatik.GetStepperByPosition(x, y);
                        ushort a = entry.Choreography.GetHeight(stepper.Cluster, time, x, y);
                        ushort b = next.Choreography.GetHeight(stepper.Cluster, time, x, y);

                        stepper.Set((ushort)MathHelper.Lerp(a, b, pos));
                    }
            }
            else
                ChoreographyDirect.ApplyFunction(kugelmatik, time, entry.Choreography);
        }
    }
}
