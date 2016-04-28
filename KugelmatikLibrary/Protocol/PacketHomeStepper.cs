using System;
using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketHomeStepper : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.HomeStepper; }
        }

        public const int MagicValue = 0xABCD;

        public StepperPosition Position;

        public PacketHomeStepper(StepperPosition position)
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

            this.Position = new StepperPosition(reader.ReadByte());
        }

        public void Write(BinaryWriter writer)
        {
            if(writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(0xABCD);
            writer.Write(Position.Value);
        }
    }
}
