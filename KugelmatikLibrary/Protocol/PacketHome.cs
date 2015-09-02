using System;
using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketHome : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Home; }
        }

        public const int MagicValue = 0xABCD;

        public void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            int magicValue = reader.ReadInt32();
            if (magicValue != MagicValue)
                throw new InvalidDataException("Unkown magic value: " + magicValue);
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(MagicValue);
        }
    }
}
