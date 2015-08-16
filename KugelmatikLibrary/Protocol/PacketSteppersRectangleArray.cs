using System;
using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketSteppersRectangleArray : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.SteppersRectangleArray; }
        }

        public readonly byte MinX;
        public readonly byte MinY;
        public readonly byte MaxX;
        public readonly byte MaxY;
        public readonly ushort[] Heights;
        public readonly byte[] WaitTimes;

        public PacketSteppersRectangleArray(byte minX, byte minY, byte maxX, byte maxY, ushort[] heights, byte[] waitTimes)
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

            if (heights == null)
                throw new ArgumentNullException("heights");

            int area = (maxX - minX + 1) * (maxY - minY + 1); // +1, da max die letzte Kugel nicht beinhaltet
            if (heights.Length != area)
                throw new ArgumentException("Heights length does not match area of rectangle.", "heights");

            if (waitTimes == null)
                throw new ArgumentNullException("waitTimes");
            if (waitTimes.Length != area)
                throw new ArgumentException("WaitTimes length does not match area of rectangle.", "waitTimes");

            this.MinX = minX;
            this.MinY = minY;
            this.MaxX = maxX;
            this.MaxY = maxY;
            this.Heights = heights;
            this.WaitTimes = waitTimes;
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write((byte)(MinX << 4 | MinY));
            writer.Write((byte)(MaxX << 4 | MaxY));
            for (int i = 0; i < Heights.Length; i++)
            {
                writer.Write(Heights[i]);
                writer.Write(WaitTimes[i]);
            }
        }
    }
}