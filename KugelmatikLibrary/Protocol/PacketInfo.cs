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

        public void Read(BinaryReader reader)
        {
        }

        public void Write(BinaryWriter writer)
        {
        }
    }
}