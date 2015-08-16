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

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(0xABCD);
        }
    }
}
