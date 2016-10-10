using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketStop : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Stop; }
        }

        public void Read(BinaryReader reader)
        {
        }

        public void Write(BinaryWriter writer)
        {
        }
    }
}