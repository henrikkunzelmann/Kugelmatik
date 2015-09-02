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

        public StepperPosition Min;
        public StepperPosition Max;
        public ushort[] Heights;
        public byte[] WaitTimes;

        public PacketSteppersRectangleArray(StepperPosition min, StepperPosition max, ushort[] heights, byte[] waitTimes)
        {
            if (max.X < min.X)
                throw new ArgumentException("Max.X is smaller then min.X.", "max.X");
            if (max.Y < min.Y)
                throw new ArgumentException("Max.Y is smaller then min.Y.", "max.Y");

            if (heights == null)
                throw new ArgumentNullException("heights");

            int area = (max.X - min.X + 1) * (max.Y - min.Y + 1); // +1, da max die letzte Kugel nicht beinhaltet
            if (area <= 0)
                throw new ArgumentOutOfRangeException("area");

            if (heights.Length != area)
                throw new ArgumentException("Heights length does not match area of rectangle.", "heights");

            if (waitTimes == null)
                throw new ArgumentNullException("waitTimes");
            if (waitTimes.Length != area)
                throw new ArgumentException("WaitTimes length does not match area of rectangle.", "waitTimes");

            this.Min = min;
            this.Max = max;
            this.Heights = heights;
            this.WaitTimes = waitTimes;
        }

        public void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            this.Min = new StepperPosition(reader);
            this.Max = new StepperPosition(reader);

            if (Max.X < Min.X)
                throw new InvalidDataException("Max.X is smaller then min.X.");
            if (Max.Y < Min.Y)
                throw new InvalidDataException("Max.Y is smaller then min.Y.");

            int area = (Max.X - Min.X + 1) * (Max.Y - Min.Y + 1); // +1, da max die letzte Kugel nicht beinhaltet
            if (area <= 0)
                throw new InvalidDataException("Area of rectangle is smaller or equal to 0.");

            this.Heights = new ushort[area];
            this.WaitTimes = new byte[area];

            for (int i = 0; i < area; i++)
            {
                this.Heights[i] = reader.ReadUInt16();
                this.WaitTimes[i] = reader.ReadByte();
            }
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(Min.Value);
            writer.Write(Max.Value);
            for (int i = 0; i < Heights.Length; i++)
            {
                writer.Write(Heights[i]);
                writer.Write(WaitTimes[i]);
            }
        }
    }
}