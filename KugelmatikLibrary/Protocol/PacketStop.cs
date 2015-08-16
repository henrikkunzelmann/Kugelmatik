using System;
using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketStop : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Stop; }
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
        }
    }
}