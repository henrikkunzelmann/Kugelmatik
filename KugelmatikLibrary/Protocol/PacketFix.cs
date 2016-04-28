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

        public const int MagicValue = 0xDCBA;

        public StepperPosition Position;

        public PacketFix(StepperPosition position)
        {
            this.Position = position;
        }

        public void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            int magicValue = reader.ReadInt32();
            if (magicValue != MagicValue)
                throw new InvalidDataException("Unknown magic value: " + magicValue);

            this.Position = new StepperPosition(reader);
        }

        public void Write(BinaryWriter writer)
        {
            if(writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(MagicValue);
            writer.Write(Position.Value);
        }
    }
}
