using KugelmatikLibrary.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace KugelmatikLibrary
{
    /// <summary>
    /// Bietet Funktionen um Cluster im Netzwerk zu suchen und in einer Liste zu sammeln.
    /// </summary>
    public class ClusterSearch : IDisposable
    {
        private Config config;
        private UdpClient client;
        private List<ClusterEntry> clusters = new List<ClusterEntry>();

        public bool IsDisposed { get; private set; }

        public ClusterSearch(Config config)
        {
            this.config = config;

            client = new UdpClient(config.ProtocolPort);
            client.EnableBroadcast = true;
            client.BeginReceive(ReceivePacket, null);
        }

        ~ClusterSearch()
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

            IsDisposed = true;

            if (disposing)
                client?.Close();
        }

        private void SendInfoBroadcast()
        {
            Log.Debug("ClusterSearch: Sending info broadcast");

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write((byte)'K');
                writer.Write((byte)'K');
                writer.Write((byte)'S');
                writer.Write((byte)0); // nicht bestätigen
                
                writer.Write((byte)PacketType.Info);
                writer.Write(0); // Revision

                PacketInfo info = new PacketInfo(true);
                info.Write(writer);

                byte[] packet = stream.GetBuffer();
                client.Send(stream.GetBuffer(), (int)stream.Length, new IPEndPoint(IPAddress.Broadcast, config.ProtocolPort));
            }
        }

        public ClusterEntry[] SearchClusters(TimeSpan timeout)
        {
            lock (clusters)
            {
                clusters.Clear();
                SendInfoBroadcast();
            }

            Thread.Sleep(timeout);
                
            lock (clusters)
            {
                return clusters.ToArray();
            }
        }

        private void ReceivePacket(IAsyncResult result)
        {
            if (IsDisposed)
                return;

            try
            {
                IPEndPoint sender = null;
                byte[] packet = client.EndReceive(result, ref sender);

                Log.Debug("ClusterSearch: Got info answer from {0}", sender.Address);
                HandlePacket(packet, sender);

                client.BeginReceive(ReceivePacket, null);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private void HandlePacket(byte[] packet, IPEndPoint sender)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(packet))
                {
                    BinaryReader reader = new BinaryReader(stream);

                    if (reader.ReadByte() != 'K' || reader.ReadByte() != 'K' || reader.ReadByte() != 'S')
                    {
                        Log.Debug("ClusterSearch: Invalid magic value within answer!");
                        return;
                    }

                    reader.ReadByte(); // bestätigen Flag ignorieren

                    if (reader.ReadByte() != (byte)PacketType.Info)
                        return;

                    reader.ReadInt32(); // Revision ignorieren

                    byte firmwareVersion = reader.ReadByte();

                    // ignorieren wenn invalide Firmeware Version
                    // und um eigenes Info Paket zu filtern, da die Info Anfrage auch wieder empfangen wird
                    if (firmwareVersion <= 1)
                        return;

                    lock (clusters)
                    {
                        // alte Einträge des Clusters entfernen
                        for (int i = 0; i < clusters.Count; i++)
                            if (clusters[i].Address == sender.Address)
                                clusters.RemoveAt(i--);

                        clusters.Add(new ClusterEntry(sender.Address, firmwareVersion));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Error while searching for clusters:");
                Log.Error(e);
            }
        }
    }
}
