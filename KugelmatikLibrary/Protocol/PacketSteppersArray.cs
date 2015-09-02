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

        public Item[] Items;

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

        public void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            byte itemCount = reader.ReadByte();
            if (itemCount == 0)
                throw new InvalidDataException("Item count is 0.");

            this.Items = new Item[itemCount];
            for (int i = 0; i < Items.Length; i++)
            {
                Items[i] = new Item(
                    new StepperPosition(reader),
                    reader.ReadUInt16(),
                    reader.ReadByte());
            }
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write((byte)Items.Length);
            for (int i = 0; i < Items.Length; i++)
            {
                Item item = Items[i];
                writer.Write(item.Position.Value);
                writer.Write(item.Height);
                writer.Write(item.WaitTime);
            }
        }

        public struct Item
        {
            public readonly StepperPosition Position;
            public readonly ushort Height;
            public readonly byte WaitTime;

            public Item(StepperPosition position, ushort height, byte WaitTime)
            {
                this.Position = position;
                this.Height = height;
                this.WaitTime = WaitTime;
            }
        }
    }
}
