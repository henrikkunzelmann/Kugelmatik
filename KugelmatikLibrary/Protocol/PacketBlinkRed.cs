using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketBlinkRed : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.BlinkRed; }
        }

        public void Read(BinaryReader reader)
        {
        }

        public void Write(BinaryWriter writer)
        {
        }
    }
}