using System;
using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketFix : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Fix; }
        }

        public readonly byte X;
        public readonly byte Y;

        public PacketFix(byte x, byte y)
        {
            if (x > 16)
                throw new ArgumentOutOfRangeException("x");
            if (y > 16)
                throw new ArgumentOutOfRangeException("y");

            this.X = x;
            this.Y = y;
        }

        public void Write(BinaryWriter writer)
        {
            if(writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(0xDCBA);
            writer.Write((byte)((X << 4) | Y));
        }
    }
}
