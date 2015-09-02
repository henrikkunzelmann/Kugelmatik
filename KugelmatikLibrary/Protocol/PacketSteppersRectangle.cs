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

        public StepperPosition Min;
        public StepperPosition Max;
        public ushort Height;
        public byte WaitTime;

        public PacketSteppersRectangle(StepperPosition min, StepperPosition max, ushort height, byte waitTime)
        {
            if (max.X < min.X)
                throw new ArgumentException("Max.X is smaller then min.X.", "max.X");
            if (max.Y < min.Y)
                throw new ArgumentException("Max.Y is smaller then min.Y.", "max.Y");

            this.Min = min;
            this.Max = max;
            this.Height = height;
            this.WaitTime = waitTime;
        }

        public void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            Min = new StepperPosition(reader);
            Max = new StepperPosition(reader);
            Height = reader.ReadUInt16();
            WaitTime = reader.ReadByte();
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(Min.Value);
            writer.Write(Max.Value);
            writer.Write(Height);
            writer.Write(WaitTime);
        }
    }
}
