using System;
using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketStepper : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Stepper; }
        }

        public readonly byte X;
        public readonly byte Y;
        public readonly ushort Height;
        public readonly byte WaitTime;

        public PacketStepper(byte x, byte y, ushort height, byte waitTime)
        {
            if (x > 16)
                throw new ArgumentOutOfRangeException("x");
            if (y > 16)
                throw new ArgumentOutOfRangeException("y");

            this.X = x;
            this.Y = y;
            this.Height = height;
            this.WaitTime = waitTime;
        }

        public void Write(BinaryWriter writer)
        {
            if(writer == null)
                throw new ArgumentNullException("writer");

            writer.Write((byte)((X << 4) | Y));
            writer.Write(Height);
            writer.Write(WaitTime);
        }
    }
}
