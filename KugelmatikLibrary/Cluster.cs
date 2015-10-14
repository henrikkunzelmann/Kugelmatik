using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using KugelmatikLibrary.Protocol;

namespace KugelmatikLibrary
{
    /// <summary>
    /// Stellt ein Cluster in der Kugelmatik dar. Jedes Cluster beinhaltet die Stepper für die Kugeln.
    /// </summary>
    public class Cluster : IDisposable
    {
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Anzahl der Stepper in der Breite (X).
        /// </summary>
        public const int Width = 5;

        /// <summary>
        /// Anzahl der Stepper in der Höhe (Y).
        /// </summary>
        public const int Height = 6;

        /// <summary>
        /// Gibt den Index des MCPs für einen Stepper an.
        /// </summary>
        private static int[] mcpIndex = { 6, 6, 5, 5, 4, 6, 6, 5, 5, 4, 7, 7, 1, 2, 2, 7, 7, 1, 2, 2, 0, 0, 1, 3, 3, 0, 0, 1, 3, 3 };

        /// <summary>
        /// Gibt den Index des Schrittmotors für jeden Stepper an. 
        /// </summary>
        private static int[] stepperIndex = { 2, 3, 2, 3, 1, 1, 0, 1, 0, 0, 1, 0, 3, 2, 3, 2, 3, 2, 1, 0, 2, 3, 1, 2, 3, 1, 0, 0, 1, 0 };

        /// <summary>
        /// Gibt die Kugelmatik-Instanze zurück zu der das Cluster gehört.
        /// </summary>
        public Kugelmatik Kugelmatik { get; private set; }

        /// <summary>
        /// Gibt die X-Koordinate der Position in der Kugelmatik zurück.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Gibt die Y-Koordiante der Position in der Kugelmatik zurück.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Array für alle Stepper die zum Cluster gehören.
        /// </summary>
        private Stepper[] steppers;

        /// <summary>
        /// Gibt die Anzahl der Stepper in diesem Cluster zurück.
        /// </summary>
        public int StepperCount
        {
            get { return steppers.Length; }
        }

        /// <summary>
        /// Gibt den Stepper mit dem Index index zurück.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Stepper this[int index]
        {
            get { return GetStepperByIndex(index); }
        }


        /// <summary>
        /// Gibt zurück ob sich mindestens ein Stepper am Cluster ändern soll.
        /// </summary>
        public bool IsInvalid
        {
            get
            {
                foreach (Stepper stepper in steppers)
                    if (stepper.IsInvalid)
                        return true;
                return false;
            }
        }

        private int lastPing = Environment.TickCount;
        private int ping = -1;
        private Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Gibt die letzte Paketumlaufzeit an die vom Cluster erfasst zurück. 
        /// Der Wert ist -1 wenn noch kein Ping-Wert emfangen wurde.
        /// </summary>
        public int Ping
        { 
            get
            {
                return ping;
            }
            private set
            {
                if (ping != value)
                {
                    ping = value;
                    if (OnPingChange != null)
                        OnPingChange(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Wird aufgerufen wenn das Cluster verbunden ist.
        /// </summary>
        public event EventHandler OnConnected;

        /// <summary>
        /// Wird aufgerufen wenn sich der Ping-Wert ändert.
        /// </summary>
        public event EventHandler OnPingChange;

        /// <summary>
        /// Gibt die aktuelle Revision der Daten an die zum Cluster geschickt wurden.
        /// </summary>
        private int currentRevision = 1;

        /// <summary>
        /// Gibt die IPAdress des Cluster zurück.
        /// </summary>
        public IPAddress Address { get; private set; }

        private ClusterInfo info;

        /// <summary>
        /// Gibt Informationen über das Cluster zurück. Null wenn noch keine Informationen empfangen wurden.
        /// </summary>
        public ClusterInfo Info
        {
            get
            {
                lock(locker)
                {
                    return info;
                }
            }
            set
            {
                lock (locker)
                {
                    if (value != info)
                    {
                        info = value;
                        if (OnInfoChange != null)
                            OnInfoChange(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Wird aufgerufen wenn sich der Info-Wert ändert.
        /// </summary>
        public event EventHandler OnInfoChange;

        /// <summary>
        /// Gibt den Socket an mit dem das Cluster mit der Hardware per UDP verbunden ist.
        /// </summary>
        private UdpClient socket;

        /// <summary>
        /// Gibt den Paket-Buffer an der benutzt wird um die Pakete zu generieren.
        /// </summary>
        private MemoryStream packetBuffer = new MemoryStream();

        /// <summary>
        /// BinaryWriter der für den Packet-Buffer zum Schreiben benutzt wird.
        /// </summary>
        private BinaryWriter packetWriter;

        /// <summary>
        /// Gibt den Zeitpunkt an als das Paket abgeschickt wurde.
        /// </summary>
        private Dictionary<int, long> packetSendTime = new Dictionary<int, long>();

        /// <summary>
        /// Packete die noch vom Cluster bestätigt werden müssen.
        /// </summary>
        private Dictionary<int, IPacket> packetsToAcknowledge = new Dictionary<int, IPacket>();

        /// <summary>
        /// Gibt zurück ob Pakete noch warten vom Cluster bestätigt zu werden.
        /// </summary>
        public bool AnyPacketsAcknowledgePending
        {
            get { return packetsToAcknowledge.Count > 0; }
        }

        /// <summary>
        /// Gibt die Anzahl der Pakete zurück die noch vom Cluster bestätigt werden müssen.
        /// </summary>
        public int PendingAcknowledgePacketsCount
        {
            get { return packetsToAcknowledge.Count; }
        }


        private object locker = new object();

        public Cluster(Kugelmatik kugelmatik, int x, int y, IPAddress address)
        {
            if (kugelmatik == null)
                throw new ArgumentNullException("kugelmatik");
            if (x < 0 || x >= 16)
                throw new ArgumentOutOfRangeException("x");
            if (y < 0 || y >= 16)
                throw new ArgumentOutOfRangeException("y");
            if (address == null)
                throw new ArgumentNullException("address");

            this.Kugelmatik = kugelmatik;
            this.X = x;
            this.Y = y;
            this.Address = address;

            steppers = new Stepper[Width * Height];
            for (byte i = 0; i < Width; i++)
                for (byte j = 0; j < Height; j++)
                    steppers[j * Width + i] = new Stepper(this, i, j);

            socket = new UdpClient();
            socket.Connect(address, kugelmatik.Config.ProtocolPort);

            packetWriter = new BinaryWriter(packetBuffer);

            socket.BeginReceive(ReceivePacket, null);

            // Ping senden und ein ResetRevision Paket senden damit die Revision wieder zurück gesetzt wird
            SendPing();

            OnConnected += (sender, args) =>
            {
                SendPacket(new PacketResetRevision(), true);
                // Daten abfragen
                SendGetData();
                SendInfo();
            };
        }

        ~Cluster()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                if (socket != null)
                    socket.Close();

                if (packetBuffer != null)
                    packetBuffer.Dispose();

                if (packetWriter != null)
                    packetWriter.Dispose();
            }

            IsDisposed = true;
        }

        public IEnumerable<Stepper> EnumerateSteppers()
        {
            for (int i = 0; i < steppers.Length; i++)
                yield return steppers[i];
        }

        /// <summary>
        /// Gibt den Stepper mit dem Index index zurück.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Stepper GetStepperByIndex(int index)
        {
            if (index < 0 || index >= steppers.Length)
                throw new ArgumentOutOfRangeException("index");
            return steppers[index];
        }

        /// <summary>
        /// Gibt einen Stepper für die Position x und y zurück.
        /// </summary>
        /// <param name="x">Die X-Koordiante der Position.</param>
        /// <param name="y">Die Y-Koordinate der Position.</param>
        /// <returns></returns>
        public Stepper GetStepperByPosition(int x, int y)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException("x");
            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException("y");

            return steppers[y * Width + x];
        }

        /// <summary>
        /// Bewegt alle Stepper zur selben Höhe.
        /// </summary>
        /// <param name="height"></param>
        public void MoveAllStepper(ushort height)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);

            for (int i = 0; i < steppers.Length; i++)
                steppers[i].MoveTo(height);
        }

        /// <summary>
        /// Sendet alle Daten an das Cluster um die Höhen zu aktualisieren.
        /// </summary>
        /// <returns>Gibt true zurück, wenn Daten an das Cluster gesendet wurden</returns>
        public bool SendData()
        {
            return SendData(false);
        }

        /// <summary>
        /// Sendet alle Daten an das Cluster um die Höhen zu aktualisieren.
        /// </summary>
        /// <param name="guaranteed">Wert der angibt ob das Paket vom Cluster bestätigt werden muss.</param>
        /// <returns>Gibt true zurück, wenn Daten an das Cluster gesendet wurden</returns>
        public bool SendData(bool guaranteed)
        {
            return SendData(guaranteed, true);
        }


        /// <summary>
        /// Sendet alle Daten an das Cluster um die Höhen zu aktualisieren.
        /// </summary>
        /// <param name="guaranteed">Wert der angibt ob das Paket vom Cluster bestätigt werden muss.</param>
        /// <param name="ignoreInvalid">Wert der angibt ob alle Stepper (auch valide) zum Cluster gesendet werden sollen.</param>
        /// <returns>Gibt true zurück, wenn Daten an das Cluster gesendet wurden</returns>
        public bool SendData(bool guaranteed, bool sendAllSteppers)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);

            lock (locker)
            {
                if (!IsInvalid)
                    return false;

                bool anyDataSent = SendDataInternal(guaranteed, sendAllSteppers);
                if (anyDataSent)
                {
                    for (int i = 0; i < steppers.Length; i++)
                        steppers[i].IsInvalid = false;
                }
                return anyDataSent;
            }
        }

        /// <summary>
        /// Verschickt alle Pakete nochmal die noch vom Cluster bestätigt werden.
        /// </summary>
        /// <returns>Gibt true zurück, wenn Pakete gesendet wurden.</returns>
        public bool ResendPendingPackets()
        {
            lock (locker)
            {
                bool anyDataSent = false;
                KeyValuePair<int, IPacket>[] packets = packetsToAcknowledge.ToArray();
                foreach (KeyValuePair<int, IPacket> packet in packets)
                    if (stopwatch.ElapsedMilliseconds - packetSendTime[packet.Key] > Math.Max(Ping, Kugelmatik.Config.AcknowlegdeTime)) // ist das Paket alt genug zum neusenden?
                        anyDataSent |= SendPacket(packet.Value, true, packet.Key);
                return anyDataSent;
            }
        }

        private bool SendDataInternal(bool guaranteed, bool sendAllSteppers)
        {
            var invalidSteppers = steppers.Where(s => sendAllSteppers || s.IsInvalid);
            if (invalidSteppers.Count() == 0)
                return false;

            // Im folgenden wird je nach Fall entschieden welches das beste Update-Packet ist um möglichst wenig senden zu müssen
            var stepperHeights = steppers.Select(s => s.Height);
            bool allSteppersSameHeight = stepperHeights.All(h => h == stepperHeights.First());

            var stepperWaitTimes = steppers.Select(s => s.WaitTime);
            bool allSteppersSameWaitTime = stepperWaitTimes.All(w => w == stepperWaitTimes.First());

            if (allSteppersSameHeight && allSteppersSameWaitTime)
                return SendPacket(new PacketAllSteppers(stepperHeights.First(), stepperWaitTimes.First()), guaranteed);

            // Schauen ob alle Stepper die selbe Höhe und selbe WaitTime haben
            var invalidSteppersHeight = invalidSteppers.Select(s => s.Height);
            bool allInvalidSteppersSameHeight = invalidSteppersHeight.All(h => h == invalidSteppersHeight.First());

            var invalidSteppersWaitTimes = invalidSteppers.Select(s => s.WaitTime);
            bool allInvalidSteppersSameWaitTime = invalidSteppersWaitTimes.All(w => w == invalidSteppersWaitTimes.First());

            // Schauen ob alle Stepper in einem Rechteck liegen 
            byte minStepperX = invalidSteppers.Select(s => s.X).Min();
            byte maxStepperX = invalidSteppers.Select(s => s.X).Max();
            byte minStepperY = invalidSteppers.Select(s => s.Y).Min();
            byte maxStepperY = invalidSteppers.Select(s => s.Y).Max();
            int rectArea = (maxStepperX - minStepperX + 1) * (maxStepperY - minStepperY + 1); // +1, da max die letzte Kugel nicht beinhaltet
            if (rectArea >= 2 && rectArea <= 8) // wenn das Rechteck 8 Schrittmotoren umfasst (und nicht z.B. das ganze Cluster)
            {
                var allSteppersInRect = invalidSteppers
                    .All(s => s.X >= minStepperX && s.X <= maxStepperX && s.Y >= minStepperY && s.Y <= maxStepperY); // schauen ob Stepper im Rechteck liegt

                if (allSteppersInRect)
                {
                    if (allInvalidSteppersSameHeight && allSteppersSameWaitTime)
                    {
                        return SendPacket(new PacketSteppersRectangle(
                            new StepperPosition(minStepperX, minStepperY),
                            new StepperPosition(maxStepperX, maxStepperY), 
                            invalidSteppersHeight.First(), 
                            invalidSteppersWaitTimes.First()), guaranteed);
                    }
                    else
                    {
                        ushort[] rectHeights = new ushort[rectArea];
                        byte[] waitTimes = new byte[rectArea];

                        int i = 0;
                        for (int x = minStepperX; x <= maxStepperX; x++)
                            for (int y = minStepperY; y <= maxStepperY; y++)
                            {
                                Stepper stepper = GetStepperByPosition(x, y);
                                rectHeights[i] = stepper.Height;
                                waitTimes[i++] = stepper.WaitTime;
                            }

                        return SendPacket(new PacketSteppersRectangleArray(
                            new StepperPosition(minStepperX, minStepperY),
                            new StepperPosition(maxStepperX, maxStepperY),
                            rectHeights, waitTimes), guaranteed);
                    }
                }
            }

            if (invalidSteppers.Count() == 1)
            {
                bool anyDataSent = false;
                foreach (Stepper stepper in invalidSteppers)
                    anyDataSent |= SendPacket(new PacketStepper(new StepperPosition(stepper), stepper.Height, stepper.WaitTime), guaranteed);
                return anyDataSent;
            }
            else if (invalidSteppers.Count() <= 10)
            {
                if (allInvalidSteppersSameHeight && allInvalidSteppersSameWaitTime)
                    return SendPacket(new PacketSteppers(invalidSteppers.Select(s => new StepperPosition(s)).ToArray(), invalidSteppersHeight.First(), invalidSteppersWaitTimes.First()), guaranteed);
                else
                    return SendPacket(new PacketSteppersArray(invalidSteppers.Select(s => new PacketSteppersArray.Item(new StepperPosition(s), s.Height, s.WaitTime)).ToArray()), guaranteed);
            }
            else
            {
                ushort[] heights = new ushort[Width * Height];
                byte[] waitTimes = new byte[Width * Height];
                int i = 0;
                // beide for-Schleifen müssen in der Reihenfolge übereinstimmen mit der Firmware, sonst stimmen die Positionen nicht mehr
                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                    {
                        Stepper stepper = GetStepperByPosition(x, y);
                        heights[i] = stepper.Height;
                        waitTimes[i++] = stepper.WaitTime;
                    }
                return SendPacket(new PacketAllSteppersArray(heights, waitTimes), guaranteed);
            }
        }

        /// <summary>
        /// Schickt einen Ping-Befehl an das Cluster. 
        /// </summary>
        public void SendPing()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (Environment.TickCount - lastPing > 5000)
                Ping = -1;

            if (!stopwatch.IsRunning)
                stopwatch.Start();

            SendPacket(new PacketPing(stopwatch.ElapsedMilliseconds), false);
        }

        /// <summary>
        /// Schickt den Home-Befehl an das Cluster und setzt alle Kugeln auf die Höhe 0.
        /// </summary>
        public void SendHome()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);

            lock (locker)
            {
                SendPacket(new PacketHome(), true);

                for (int i = 0; i < steppers.Length; i++)
                {
                    Stepper stepper = steppers[i];
                    stepper.Height = 0;
                    stepper.IsInvalid = false;
                }
            }
        }

        /// <summary>
        /// Schickt einen GetData-Befehl an das Cluster.
        /// </summary>
        public void SendGetData()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);

            SendPacket(new PacketGetData(), false);
        }

        /// <summary>
        /// Schickt einen Info-Befehl an das Cluster.
        /// </summary>
        public void SendInfo()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);

            SendPacket(new PacketInfo(), true);
        }

        /// <summary>
        /// Schickt einen Config-Befehl an das Cluster.
        /// </summary>
        public void SendConfig(ClusterConfig config)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);

            SendPacket(new PacketConfig(config), true);
        }

        /// <summary>
        /// Schickt einen Stop-Befehl an das Cluster.
        /// </summary>
        public void SendStop()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);

            SendPacket(new PacketStop(), true);
        }

        /// <summary>
        /// Schickt ein Packet an das Cluster.
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="guaranteed">Ob vom Cluster eine Antwort gefordert wird.</param>
        public bool SendPacket(IPacket packet, bool guaranteed)
        {
            return SendPacket(packet, guaranteed, currentRevision++);
        }

        private bool SendPacket(IPacket packet, bool guaranteed, int revision)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
            if (packet == null)
                throw new ArgumentNullException("packet");

            // wenn das Cluster nicht erreichbar ist
            if (Ping < 0)
            {
                if (Kugelmatik.Config.IgnoreGuaranteedWhenOffline)
                    guaranteed = false;

                // alle Pakete (außer Ping) ignorieren wenn das Cluster offline ist
                if (packet.Type != PacketType.Ping && Kugelmatik.Config.IgnorePacketsWhenOffline)
                    return false;
            }

            if (Kugelmatik.Config.IgnoreGuaranteed)
                guaranteed = false;

            lock (locker)
            {
                bool alreadySent = packetsToAcknowledge.ContainsKey(revision);
                if (guaranteed)
                {
                    if (!stopwatch.IsRunning)
                        stopwatch.Start();
                    packetsToAcknowledge[revision] = packet;
                    packetSendTime[revision] = stopwatch.ElapsedMilliseconds;
                }

                packetBuffer.Position = 0;

                // Paket-Header schreiben
                packetWriter.Write((byte)'K');
                packetWriter.Write((byte)'K');
                packetWriter.Write((byte)'S');

                // wenn das Cluster eine Antwort schickt dann wird kein Ack-Paket angefordert, sonst kann es passieren, dass das Ack-Paket die eigentliche Antwort verdrängt
                if (!packet.Type.DoesClusterAnswer() && guaranteed)
                    packetWriter.Write((byte)1);
                else
                    packetWriter.Write((byte)0);

                packetWriter.Write((byte)packet.Type);
                packetWriter.Write(revision);

                // Paket Inhalt schreiben
                packet.Write(packetWriter);


                socket.BeginSend(packetBuffer.GetBuffer(), (int)packetBuffer.Position, SendPacket, null);
                if (Kugelmatik.Config.VerbosePacketSending && (packet.Type != PacketType.Ping || Kugelmatik.Config.LogPingPacket))
                    Log.Verbose("[{0}, {1}] Packet:   [{2}] {3}, size: {4} bytes {5} {6}", X, Y, revision, packet.Type, packetBuffer.Position, guaranteed ? "(guaranteed)" : "", alreadySent ? "(resend)" : "");
            }
            return true;
        }

        private void SendPacket(IAsyncResult result)
        {
            try
            {
                socket.EndSend(result);
            }
            catch(SocketException)
            {
                // Cluster ist möglicherweiße nicht verfügbar
            }
        }

        private void ReceivePacket(IAsyncResult result)
        {
            if (IsDisposed)
                return;

            try
            {
                IPEndPoint endPoint = new IPEndPoint(Address, Kugelmatik.Config.ProtocolPort);
                byte[] packet = socket.EndReceive(result, ref endPoint);

                // kein Packet empfangen
                if (packet == null || packet.Length == 0)
                {
                    socket.BeginReceive(ReceivePacket, null);
                    return;
                }

                lock (locker)
                {
                    HandlePacket(packet);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                if (Debugger.IsAttached)
                    Debugger.Break();
            }
            finally
            {
                socket.BeginReceive(ReceivePacket, null);
            }
        }

        private const int HeaderSize = 9; // Bytes

        private void HandlePacket(byte[] packet)
        {
            // jedes Kugelmatik V3 Paket ist mindestens HeaderSize Bytes lang und fangen mit "KKS" an
            if (packet.Length < HeaderSize || packet[0] != 'K' || packet[1] != 'K' || packet[2] != 'S')
                return;

            bool isGuaranteed = packet[3] > 0;
            PacketType type = (PacketType)packet[4];

            using (MemoryStream stream = new MemoryStream(packet))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                stream.Position = HeaderSize - 4; // 4 Bytes für die Revision

                int revision = reader.ReadInt32();

                if (Kugelmatik.Config.VerbosePacketReceive 
                    && type != PacketType.Ack 
                    && (type != PacketType.Ping || Kugelmatik.Config.LogPingPacket))
                    Log.Verbose("[{0}, {1}] Received: [{2}] {3}, size: {4} bytes", X, Y, revision, type, packet.Length);

                switch (type)
                {
                    case PacketType.Ping:
                        if (packet.Length < HeaderSize + sizeof(long))
                            throw new InvalidDataException("Packet is not long enough.");

                        int timeSpan = Environment.TickCount - lastPing;
                        if (timeSpan > 1000 * 10  || ping < 0)
                            if (OnConnected != null)
                                OnConnected(this, EventArgs.Empty);

                        lastPing = Environment.TickCount;

                        long time = reader.ReadInt64(); // time ist der Wert von stopwatch zum Zeitpunkt des Absenden des Pakets
                        Ping = (int)(stopwatch.ElapsedMilliseconds - time);

                        RemovePacketToAcknowlegde(revision);
                        break;
                    case PacketType.Ack:
                        IPacket acknowlegdedPacket;
                        if (!packetsToAcknowledge.TryGetValue(revision, out acknowlegdedPacket))
                        {
                            if (Kugelmatik.Config.VerbosePacketReceive)
                                Log.Verbose("[{0}, {1}] Unkown acknowlegde: [{2}]", X, Y, revision);
                            break;
                        }

                        if (Kugelmatik.Config.VerbosePacketReceive)
                            Log.Verbose("[{0}, {1}] Acknowlegde: [{2}] {3}", X, Y, revision, acknowlegdedPacket.Type);

                        RemovePacketToAcknowlegde(revision);
                        break;
                    case PacketType.GetData:
                        if (packet.Length < HeaderSize + 3 * Width * Height)
                            throw new InvalidDataException("Packet is not long enough.");

                        for (int x = 0; x < Width; x++) // for-Schleife muss mit Firmware übereinstimmen
                            for (int y = 0; y < Height; y++)
                            {
                                Stepper stepper = GetStepperByPosition(x, y);

                                ushort height = reader.ReadUInt16(); 
                                if (height > Kugelmatik.Config.MaxHeight)
                                    continue; // Höhe ignorieren

                                byte waitTime = reader.ReadByte();
                                stepper.Height = height;
                                stepper.WaitTime = waitTime;
                            }
                        RemovePacketToAcknowlegde(revision);
                        break;
                    case PacketType.Info:
                        if (packet.Length < HeaderSize + 6)
                            throw new InvalidDataException("Packet is not long enough.");

                        byte buildVersion = reader.ReadByte();

                        BusyCommand currentBusyCommand = BusyCommand.None;
                        if (buildVersion >= 11)
                        {
                            if (packet.Length < HeaderSize + 8)
                                throw new InvalidDataException("Packet is not long enough.");
                            currentBusyCommand = (BusyCommand)reader.ReadByte();
                        }
                        else if(buildVersion >= 8)
                        {
                            if (packet.Length < HeaderSize + 8)
                                throw new InvalidDataException("Packet is not long enough.");
                            currentBusyCommand = reader.ReadByte() > 0 ? BusyCommand.Unkown : BusyCommand.None;
                        }

                        int highestRevision = 0;
                        if (buildVersion >= 9)
                        {
                            if (packet.Length < HeaderSize + 12)
                                throw new InvalidDataException("Packet is not long enough.");
                            highestRevision = reader.ReadInt32();
                        }

                        byte stepMode = reader.ReadByte();
                        if (!Enum.IsDefined(typeof(StepMode), stepMode))
                            throw new InvalidDataException("Unkown step mode");

                        int delayTime = reader.ReadInt32();

                        bool useBreak = false;
                        if (buildVersion >= 6)
                        {
                            if (packet.Length < HeaderSize + 7)
                                throw new InvalidDataException("Packet is not long enough.");
                            useBreak = reader.ReadByte() > 0;
                        }

                        Info = new ClusterInfo(buildVersion, currentBusyCommand, highestRevision, new ClusterConfig((StepMode)stepMode, delayTime, useBreak));
                        RemovePacketToAcknowlegde(revision);
                        break;
                    default:
                        throw new InvalidDataException("Invalid packet type to get sent by cluster.");
                }
            }
        }

        /// <summary>
        /// Entfernt ein Paket aus der Liste der noch zu bestätigen Pakete.
        /// </summary>
        /// <param name="packetID"></param>
        private void RemovePacketToAcknowlegde(int packetID)
        {
            packetsToAcknowledge.Remove(packetID);
            packetSendTime.Remove(packetID);
        }
    }
}
