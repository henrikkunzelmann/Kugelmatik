using System;
using System.IO;
using System.Net;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketAllSteppersArray : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.AllSteppersArray; }
        }

        public ushort[] Heights;
        public byte[] WaitTimes;

        public PacketAllSteppersArray(ushort[] heights, byte[] waitTimes)
        {
            if (heights == null)
                throw new ArgumentNullException("heights");
            if (heights.Length != Cluster.Width * Cluster.Height)
                throw new ArgumentException(string.Format("Heights length must match {0}.", Cluster.Width * Cluster.Height), "heights");
            if (waitTimes == null)
                throw new ArgumentNullException("waitTimes");
            if (waitTimes.Length != Cluster.Width * Cluster.Height)
                throw new ArgumentException(string.Format("WaitTimes length must match {0}.", Cluster.Width * Cluster.Height), "waitTimes");

            this.Heights = heights;
            this.WaitTimes = waitTimes;
        }

        public void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            Heights = new ushort[Cluster.Width * Cluster.Height];
            WaitTimes = new byte[Cluster.Width * Cluster.Height];

            for (int i = 0; i < Heights.Length; i++)
            {
                Heights[i] = reader.ReadUInt16();
                WaitTimes[i] = reader.ReadByte();
            }
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            for (int i = 0; i < Heights.Length; i++)
            {
                writer.Write(Heights[i]);
                writer.Write(WaitTimes[i]);
            }
        }
    }
}
