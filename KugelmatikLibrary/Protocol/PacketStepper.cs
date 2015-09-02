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

        public StepperPosition Position;
        public ushort Height;
        public byte WaitTime;

        public PacketStepper(StepperPosition position, ushort height, byte waitTime)
        {
            this.Position = position;
            this.Height = height;
            this.WaitTime = waitTime;
        }

        public void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            this.Position = new StepperPosition(reader);
            this.Height = reader.ReadUInt16();
            this.WaitTime = reader.ReadByte();
        }

        public void Write(BinaryWriter writer)
        {
            if(writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(Position.Value);
            writer.Write(Height);
            writer.Write(WaitTime);
        }
    }
}
