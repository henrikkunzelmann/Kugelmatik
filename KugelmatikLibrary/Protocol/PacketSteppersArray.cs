using System;
using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketSteppersArray : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.SteppersArray; }
        }

        public readonly Item[] Items;

        public PacketSteppersArray(Item[] items)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            if (items.Length == 0)
                throw new ArgumentException("Items is empty.", "items");
            if (items.Length > byte.MaxValue)
                throw new ArgumentException("More then " + byte.MaxValue + " items in array.", "items");

            this.Items = items;
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write((byte)Items.Length);
            for (int i = 0; i < Items.Length; i++)
            {
                Item item = Items[i];
                writer.Write((byte)((item.X << 4) | item.Y));
                writer.Write(item.Height);
                writer.Write(item.WaitTime);
            }
        }

        public struct Item
        {
            public readonly byte X;
            public readonly byte Y;
            public readonly ushort Height;
            public readonly byte WaitTime;

            public Item(byte x, byte y, ushort height, byte WaitTime)
            {
                if (x > 16)
                    throw new ArgumentOutOfRangeException("x");
                if (y > 16)
                    throw new ArgumentOutOfRangeException("y");

                this.X = x;
                this.Y = y;
                this.Height = height;
                this.WaitTime = WaitTime;
            }
        }
    }
}
