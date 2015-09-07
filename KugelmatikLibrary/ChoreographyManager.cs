using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace KugelmatikLibrary
{
    /// <summary>
    /// Verwaltet eine Choreographie und lässt diese auf der Kugelmatik abspielen.
    /// </summary>
    public class ChoreographyManager : IDisposable
    {
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gibt den Bilder pro Sekunde Wert der benutzt werden sollen um die Choreographie abzuspielen zurürck.
        /// </summary>
        public int TargetFPS { get; private set; }

        /// <summary>
        /// Gibt den derzeitigen Bilder pro Sekunden Wert mit dem die Choreographie abgespielt wird zurück.
        /// </summary>
        public int FPS { get; private set; }

        /// <summary>
        /// Gibt die Kugelmatik auf der die Choreographie abgespielt wird zurück.
        /// </summary>
        public Kugelmatik Kugelmatik { get; private set; }

        /// <summary>
        /// Gibt die Choreographie die verwaltet und abgespielt wird zurück.
        /// </summary>
        public IChoreography Choreography { get; private set; }

        /// <summary>
        /// Gibt zurück ob der ChoreographyManager läuft.
        /// </summary>
        public bool IsRunning
        {
            get { return task != null && !cancellationToken.IsCancellationRequested; }
        }

        private CancellationTokenSource cancellationToken;
        private Task task;


        public ChoreographyManager(Kugelmatik kugelmatik, int targetFPS, IChoreography choreography)
        {
            if (kugelmatik == null)
                throw new ArgumentNullException("kugelmatik");
            if (targetFPS <= 0)
                throw new ArgumentOutOfRangeException("targetFPS");
            if (choreography == null)
                throw new ArgumentNullException("choreography");

            this.Kugelmatik = kugelmatik;
            this.TargetFPS = targetFPS;
            this.Choreography = choreography;
        }

        ~ChoreographyManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                if (cancellationToken != null)
                    cancellationToken.Dispose();
            }

            IsDisposed = true;
        }

        public void Start()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
            if (task != null)
                throw new InvalidOperationException("Already running.");

            cancellationToken = new CancellationTokenSource();
            task = new Task(Run, cancellationToken.Token);
            task.Start();
        }

        public void Stop()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
            if (cancellationToken.IsCancellationRequested)
                throw new InvalidOperationException("Already canceled");

            cancellationToken.Cancel();
        }

        private void Run()
        {
            // Alle Kugeln auf Anfang bewegen lassen
            for (int x = 0; x < Kugelmatik.Config.KugelmatikWidth * Cluster.Width; x++)
                for (int y = 0; y < Kugelmatik.Config.KugelmatikHeight * Cluster.Height; y++)
                    Kugelmatik.GetStepperByPosition(x, y).MoveTo(Choreography.GetHeight(Kugelmatik.Config, TimeSpan.Zero, x, y));
            Kugelmatik.SendData(true);
            while (Kugelmatik.AnyPacketsAcknowledgePending)
            {
                Thread.Sleep(500);
                Kugelmatik.ResendPendingPackets();
            }

            Thread.Sleep(5000); // warten bis alle Kugeln auf Anfang sind

            // Zeit messen
            Stopwatch time = new Stopwatch();
            time.Start();
            Stopwatch frame = new Stopwatch();
            frame.Start();

            while (!cancellationToken.IsCancellationRequested)
            {
                TimeSpan timeStamp = time.Elapsed;
                for (int x = 0; x < Kugelmatik.Config.KugelmatikWidth * Cluster.Width; x++)
                    for (int y = 0; y < Kugelmatik.Config.KugelmatikHeight * Cluster.Height; y++)
                        Kugelmatik.GetStepperByPosition(x, y).MoveTo(Choreography.GetHeight(Kugelmatik.Config, timeStamp, x, y));
                Kugelmatik.SendData();

                if ((int)timeStamp.TotalSeconds % 2 == 0)
                    Kugelmatik.SendPing();

                int sleepTime = (int)(1000f / TargetFPS) - (int)frame.ElapsedMilliseconds; // berechnen wie lange der Thread schlafen soll um die TargetFPS zu erreichen
                if (sleepTime > 0)
                    Thread.Sleep(sleepTime);

                FPS = (int)Math.Ceiling(1000f / frame.ElapsedMilliseconds);
                frame.Restart();
            }
        }
    }
}
