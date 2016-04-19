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

        public bool RequestConfig2;

        public PacketInfo(bool requestConfig2)
        {
            this.RequestConfig2 = requestConfig2;
        }

        public void Read(BinaryReader reader)
        {
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(RequestConfig2);
        }
    }
}