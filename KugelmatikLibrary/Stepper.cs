using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KugelmatikLibrary.Protocol;

namespace KugelmatikLibrary
{
    /// <summary>
    /// Stellt einen Schrittmotor auf einem Cluster dar. Jede Kugel hat einen Schrittmotor.
    /// </summary>
    public class Stepper
    {
        /// <summary>
        /// Gibt das Cluster zurück zu dem der Schrittmotor gehört.
        /// </summary>
        public Cluster Cluster { get; private set; }

        /// <summary>
        /// Gibt die Kugelmatik zurück zu der der Schrittmotor gehört.
        /// </summary>
        public Kugelmatik Kugelmatik
        {
            get { return Cluster.Kugelmatik; }
        }

        /// <summary>
        /// Gibt die X-Koordinate der Position auf dem Cluster zurück.
        /// </summary>
        public byte X { get; private set; }

        /// <summary>
        /// Gibt die Y-Koordiante der Position auf dem Cluster zurück.
        /// </summary>
        public byte Y { get; private set; }

        private ushort lastHeight;
        private ushort height;

        /// <summary>
        /// Gibt die letzte Höhe zu der die Kugel fahren sollte zurück.
        /// </summary>
        public ushort Height
        {
            get { return height; }
            internal set
            {
                if (value >  Kugelmatik.ClusterConfig.MaxSteps)
                    throw new ArgumentOutOfRangeException();

                if (height != value)
                {
                    this.height = value;
                    if (OnHeightChange != null)
                        OnHeightChange(this, new EventArgs());
                }
            }
        }

        private byte lastWaitTime;

        /// <summary>
        /// Gibt die letzte Wartezeit zwischen jedem Tick zurück die der Schrittmotor warten sollte.
        /// </summary>
        public byte WaitTime { get; internal set; }

        /// <summary>
        /// Wird aufgerufen, wenn sich die Höhe der Kugel ändert.
        /// </summary>
        public event EventHandler OnHeightChange;

        /// <summary>
        /// Gibt den Zustand ob sich der Stepper geändert hat und Daten neugesendet werden müssen zurück.
        /// </summary>
        public bool IsInvalid
        {
            get
            {
                return lastHeight != Height || lastWaitTime != WaitTime;
            }
        }

        private object locker = new object();

        internal Stepper(Cluster cluster, byte x, byte y)
        {
            if (cluster == null)
                throw new ArgumentNullException("cluster");
            if (x < 0 || x >= Cluster.Width)
                throw new ArgumentOutOfRangeException("x");
            if (y < 0 || y >= Cluster.Height)
                throw new ArgumentOutOfRangeException("y");

            this.Cluster = cluster;
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Daten von der Cluster Klasse gesendet wurden.
        /// </summary>
        internal void OnDataSent()
        {
            lastHeight = Height;
            lastWaitTime = WaitTime;
        }

        /// <summary>
        /// Setzt den Stepper auf Anfangswerte zurück.
        /// </summary>
        private void Reset()
        {
            lastWaitTime = 0;
            WaitTime = 0;
            lastHeight = 0;
            Height = 0;
        }

        /// <summary>
        /// Bewegt die Kugel auf die Tiefe height.
        /// </summary>
        /// <param name="height">Die Tiefe zu der die Kugel fahren soll.</param>
        public void Set(ushort height)
        {
            Set(height, (byte)Kugelmatik.Config.WaitTime);
        }

        /// <summary>
        /// Bewegt die Kugel auf die Tiefe height.
        /// </summary>
        /// <param name="height">Die Tiefe zu der die Kugel fahren soll.</param>
        /// <param name="waitTime">Die Wartezeit die der Schrittmotor zwischen jedem Tick warten soll.</param>
        public void Set(ushort height, byte waitTime)
        {
            if (height > Cluster.Kugelmatik.ClusterConfig.MaxSteps)
                throw new ArgumentOutOfRangeException("height");

            lock(locker)
            {
                this.Height = height;
                this.WaitTime = waitTime;
            }
        }

        /// <summary>
        /// Bewegt die Kugel komplett nach oben.
        /// Nicht im Normalfall benutzen.
        /// </summary>
        public void SendHome()
        {
            Cluster.SendPacket(new PacketHomeStepper(new StepperPosition(this)), true);
            Reset();
        }

        /// <summary>
        /// Bewegt die Kugel komplett nach unten und dann wieder nach unten um Überläufe der Schnur zu beheben. 
        /// Nicht im Normalfall benutzen.
        /// </summary>
        public void SendFix() 
        {
            Cluster.SendPacket(new PacketFix(new StepperPosition(this)), true);
            Reset();
        }
    }
}
