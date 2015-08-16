using System;
using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketBlinkRed : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.BlinkRed; }
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
        }
    }
}