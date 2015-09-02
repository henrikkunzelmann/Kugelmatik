using System;
using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketSteppers : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Steppers; }
        }

        public StepperPosition[] Items;
        public ushort Height;
        public byte WaitTime;

        public PacketSteppers(StepperPosition[] items, ushort height, byte waitTime)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            if (items.Length == 0)
                throw new ArgumentException("Items is empty.", "items");
            if (items.Length > byte.MaxValue)
                throw new ArgumentException("More then " + byte.MaxValue + " items in array.", "items");

            this.Items = items;
            this.Height = height;
            this.WaitTime = waitTime;
        }

        public void Read(BinaryReader reader)
        {
            int itemCount = reader.ReadByte();
            if (itemCount == 0)
                throw new InvalidDataException("Item count is 0.");

            this.Height = reader.ReadUInt16();
            this.WaitTime = reader.ReadByte();

            Items = new StepperPosition[itemCount];
            for (int i = 0; i < itemCount; i++)
                Items[i] = new StepperPosition(reader);
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write((byte)Items.Length);
            writer.Write(Height);
            writer.Write(WaitTime);
            for (int i = 0; i < Items.Length; i++)
                writer.Write(Items[i].Value);
        }
    }
}
