using System;
using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketBlinkGreen : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.BlinkGreen; }
        }

        public void Read(BinaryReader reader)
        {
        }

        public void Write(BinaryWriter writer)
        {
        }
    }
}