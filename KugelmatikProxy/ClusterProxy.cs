using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.IO;
using KugelmatikLibrary;
using KugelmatikLibrary.Protocol;

namespace KugelmatikProxy
{
    public class ClusterProxy
    {
        public const int ProtocolPort = 14804;

        public ClusterInfo Info { get; private set; }

        /// <summary>
        /// Stellt den Socket für den Empfang der Daten dar.
        /// </summary>
        private UdpClient server;

        private IPEndPoint proxyEndPoint;

        /// <summary>
        /// Stellt den Socket für das Weitersenden der Daten an ein Cluster dar.
        /// </summary>
        private UdpClient proxy;

        public ClusterProxy()
        {
            server = new UdpClient(ProtocolPort);
            server.BeginReceive(ReceivePacket, null);
        }

        public void ConnectProxy(IPAddress ip)
        {
            if (ip == null)
                throw new ArgumentNullException("ip");

            if (proxy != null)
                proxy.Close();

            proxyEndPoint = new IPEndPoint(ip, ProtocolPort);
            proxy = new UdpClient(proxyEndPoint);
            proxy.BeginReceive(ReceivePacketProxy, null);
        }

        private void SendPacketCallback(IAsyncResult result)
        {
            try
            {
                server.EndSend(result);
            }
            catch (SocketException)
            {
                // Client möglicherweiße nicht erreichbar
            }
        }

        private void SendPacketProxyCallback(IAsyncResult result)
        {
            try
            {
                proxy.EndSend(result);
            }
            catch (SocketException)
            {
                // Cluster möglicherweiße nicht erreichbar
            }
        }

        private void ReceivePacket(IAsyncResult result)
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, ProtocolPort);
                byte[] packet = server.EndReceive(result, ref endPoint);

                if (proxy != null)
                    proxy.BeginSend(packet, packet.Length, SendPacketProxyCallback, null);

                HandlePacket(packet, endPoint, false);
            }
            catch (Exception e)
            {
                Log.Error("Exception: {0}", e);
                if (Debugger.IsAttached)
                    Debugger.Break();
            }
            finally
            {
                server.BeginReceive(ReceivePacket, null);
            }
        }

        private void ReceivePacketProxy(IAsyncResult result)
        {
            try
            {
                byte[] packet = proxy.EndReceive(result, ref proxyEndPoint);

                server.BeginSend(packet, packet.Length, SendPacketCallback, null);

                HandlePacket(packet, proxyEndPoint, true);
            }
            catch (Exception e)
            {
                Log.Error("Exception: {0}", e);
                if (Debugger.IsAttached)
                    Debugger.Break();
            }
            finally
            {
                proxy.BeginReceive(ReceivePacketProxy, null);
            }
        }

        private void HandlePacket(byte[] data, IPEndPoint sender, bool isFromCluster)
        {
            if (data.Length < 3)
                throw new InvalidDataException("Packet is not long enough.");
            if (data[0] != 'K' || data[1] != 'K' || data[2] != 'S')
                throw new InvalidDataException("Packet does not begin with 'KKS'.");

            using (MemoryStream stream = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                stream.Position = 3;

                bool guranteed = reader.ReadByte() > 0;
                byte packetType = reader.ReadByte();
                int rev = reader.ReadInt32();

                if (!Enum.IsDefined(typeof(PacketType), packetType))
                {
                    if (isFromCluster)
                        Log.Debug("[Cluster] [{0}] [rev: {1}{2}]", "unkown: " + packetType, rev, guranteed ? " guaranteed" : "");
                    else
                        Log.Debug("[{0}] [rev: {1}{2}]", "unkown: " + packetType, rev, guranteed ? " guaranteed" : "");
                    return;
                }

                if (!isFromCluster)
                {
                    IPacket packet = PacketFactory.CreatePacket((PacketType)packetType);
                    Log.Debug("[{0}] [rev: {1}{2}]", packet.Type, rev, guranteed ? " guaranteed" : "");


                    packet.Read(reader);
                    Log.WriteFields(LogLevel.Verbose, packet);

                    HandlePacket(data, sender, packet);

                    if (guranteed)
                    {
                        using (MemoryStream ackData = new MemoryStream())
                        using (BinaryWriter ackWriter = new BinaryWriter(ackData))
                        {
                            ackWriter.Write('K');
                            ackWriter.Write('K');
                            ackWriter.Write('S');
                            ackWriter.Write((byte)0);
                            ackWriter.Write((byte)2);
                            ackWriter.Write(rev);

                            server.BeginSend(ackData.GetBuffer(), (int)ackData.Position, SendPacketCallback, null);
                        }
                    }
                }
                else
                    Log.Debug("[Cluster] [{0}] [rev: {1}{2}]", (PacketType)packetType, rev, guranteed ? " guaranteed" : "");
            }
        }

        private void HandlePacket(byte[] data, IPEndPoint sender, IPacket packet)
        {
            switch(packet.Type)
            {
                case PacketType.Ping:
                    server.BeginSend(data, data.Length, sender, SendPacketCallback, null);
                    break;
            }
        }
    }
}
