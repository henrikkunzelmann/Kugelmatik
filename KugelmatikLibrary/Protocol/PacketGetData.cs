using System;
using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketGetData : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.GetData; }
        }

        public void Read(BinaryReader reader)
        {
        }

        public void Write(BinaryWriter writer)
        {
        }
    }
}