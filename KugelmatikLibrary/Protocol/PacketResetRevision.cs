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
        
        public void Read(BinaryReader reader)
        {

        }

        public void Write(BinaryWriter writer)
        {
        }
    }
}
