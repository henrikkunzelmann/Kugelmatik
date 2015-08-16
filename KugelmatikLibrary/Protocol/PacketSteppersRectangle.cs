using System;
using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketSteppersRectangle : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.SteppersRectangle; }
        }

        public readonly byte MinX;
        public readonly byte MinY;
        public readonly byte MaxX;
        public readonly byte MaxY;
        public readonly ushort Height;
        public readonly byte WaitTime;

        public PacketSteppersRectangle(byte minX, byte minY, byte maxX, byte maxY, ushort height, byte waitTime)
        {
            if (minX > 16)
                throw new ArgumentOutOfRangeException("minX");
            if (minY > 16)
                throw new ArgumentOutOfRangeException("minY");

            if (maxX > 16)
                throw new ArgumentOutOfRangeException("maxX");
            if (maxX < minX)
                throw new ArgumentException("MaxX is smaller then minX.", "maxX");

            if (maxY > 16)
                throw new ArgumentOutOfRangeException("maxY");
            if (maxY < minY)
                throw new ArgumentException("MaxY is smaller then minY.", "maxY");

            this.MinX = minX;
            this.MinY = minY;
            this.MaxX = maxX;
            this.MaxY = maxY;
            this.Height = height;
            this.WaitTime = waitTime;
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write((byte)(MinX << 4 | MinY));
            writer.Write((byte)(MaxX << 4 | MaxY));
            writer.Write(Height);
            writer.Write(WaitTime);
        }
    }
}
