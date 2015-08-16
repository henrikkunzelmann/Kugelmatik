using System;
using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public class PacketResetRevision : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.ResetRevision; }
        }

        public PacketResetRevision()
        {
        }

        public void Write(BinaryWriter writer)
        {
        }
    }
}
