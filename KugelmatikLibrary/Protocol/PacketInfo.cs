using System;
using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketInfo : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Info; }
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
        }
    }
}