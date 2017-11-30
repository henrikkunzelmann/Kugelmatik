using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public class PacketRestart : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Restart; }
        }

        public PacketRestart()
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
