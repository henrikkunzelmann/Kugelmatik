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

        public readonly Item[] Items;
        public readonly ushort Height;
        public readonly byte WaitTime;

        public PacketSteppers(Item[] items, ushort height, byte waitTime)
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

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write((byte)Items.Length);
            writer.Write(Height);
            writer.Write(WaitTime);
            for (int i = 0; i < Items.Length; i++)
            {
                Item item = Items[i];
                writer.Write((byte)((item.X << 4) | item.Y));
            }
        }

        public struct Item
        {
            public readonly byte X;
            public readonly byte Y;

            public Item(byte x, byte y)
            {
                if (x > 16)
                    throw new ArgumentOutOfRangeException("x");
                if (y > 16)
                    throw new ArgumentOutOfRangeException("y");

                this.X = x;
                this.Y = y;
            }
        }
    }
}
