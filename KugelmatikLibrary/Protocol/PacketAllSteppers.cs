using System;
using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketAllSteppers : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.AllSteppers; }
        }

        public ushort Height;
        public byte WaitTime;

        public PacketAllSteppers(ushort height, byte waitTime)
        {
            this.Height = height;
            this.WaitTime = waitTime;
        }

        public void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            this.Height = reader.ReadUInt16();
            this.WaitTime = reader.ReadByte();
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(Height);
            writer.Write(WaitTime);
        }
    }
}
