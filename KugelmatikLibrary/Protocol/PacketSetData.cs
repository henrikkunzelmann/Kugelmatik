using System;
using System.IO;
using System.Net;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketSetData : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.SetData; }
        }

        public ushort[] Heights;

        public PacketSetData(ushort[] heights)
        {
            if (heights == null)
                throw new ArgumentNullException("heights");
            if (heights.Length != Cluster.Width * Cluster.Height)
                throw new ArgumentException(string.Format("Heights length must match {0}.", Cluster.Width * Cluster.Height), "heights");

            this.Heights = heights;
        }

        public void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            Heights = new ushort[Cluster.Width * Cluster.Height];

            for (int i = 0; i < Heights.Length; i++)
                Heights[i] = reader.ReadUInt16();
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            for (int i = 0; i < Heights.Length; i++)
                writer.Write(Heights[i]);
        }
    }
}
