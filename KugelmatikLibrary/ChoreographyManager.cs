using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

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
        public Choreography Choreography { get; private set; }

        /// <summary>
        /// Gibt zurück ob der ChoreographyManager läuft.
        /// </summary>
        public bool IsRunning
        {
            get { return task != null && !cancellationToken.IsCancellationRequested; }
        }

        private CancellationTokenSource cancellationToken;
        private Task task;

        public ChoreographyManager(Kugelmatik kugelmatik, int targetFPS, IChoreographyFunction choreography)
            : this(kugelmatik, targetFPS, new ChoreographyDirect(choreography))
        {
            
        }

        public ChoreographyManager(Kugelmatik kugelmatik, int targetFPS, Choreography choreography)
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
                if (task != null)
                    task.Dispose();
            }

            IsDisposed = true;
        }

        /// <summary>
        /// Startet den ChoreographyManager.
        /// </summary>
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

        /// <summary>
        /// Stoppt den ChoreographyManager.
        /// </summary>
        public void Stop()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
            if (cancellationToken.IsCancellationRequested)
                throw new InvalidOperationException("Already canceled");

            cancellationToken.Cancel();
            task.Wait(TimeSpan.FromSeconds(2));
        }

        private void Run()
        {
            try
            {
                Choreography.Tick(Kugelmatik, TimeSpan.Zero);
                Kugelmatik.SendData(true);

                Stopwatch timeoutPending = new Stopwatch();
                timeoutPending.Start();

                // warten bis alle Pakete bestätigt wurden
                while (Kugelmatik.AnyPacketsAcknowledgePending)
                {
                    Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken.Token).Wait();
                    Kugelmatik.ResendPendingPackets();

                    // schauen ob wir stoppen sollen oder wir zu lange auf die Pakete gewartet haben
                    if (cancellationToken.IsCancellationRequested || timeoutPending.Elapsed > TimeSpan.FromSeconds(3))
                        return;
                }

                Task.Delay(TimeSpan.FromSeconds(5), cancellationToken.Token).Wait(); // warten bis alle Kugeln auf Anfang sind

                // Zeit messen
                Stopwatch time = new Stopwatch(); // gesammte Zeit
                time.Start();
                Stopwatch frame = new Stopwatch(); // Zeit zwischen zwei Frames
                frame.Start();

                while (!cancellationToken.IsCancellationRequested)
                {
                    TimeSpan timeStamp = time.Elapsed;

                    // Daten setzen und zu den Clustern senden
                    Choreography.Tick(Kugelmatik, timeStamp);
                    Kugelmatik.SendData();

                    // alle 2 Sekunden Ping senden
                    if ((int)timeStamp.TotalSeconds % 2 == 0)
                        Kugelmatik.SendPing();

                    // berechnen wie lange der Thread schlafen soll um die TargetFPS zu erreichen
                    int sleepTime = (int)(1000f / Math.Max(1, TargetFPS)) - (int)frame.ElapsedMilliseconds;
                    if (sleepTime > 0)
                        Thread.Sleep(sleepTime);

                    FPS = (int)Math.Ceiling(1000f / frame.ElapsedMilliseconds);
                    frame.Restart();
                }
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
