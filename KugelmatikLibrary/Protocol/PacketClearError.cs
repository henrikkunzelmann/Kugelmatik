using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketClearError : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.ClearError; }
        }

        public void Read(BinaryReader reader)
        {
        }

        public void Write(BinaryWriter writer)
        {
        }
    }
}