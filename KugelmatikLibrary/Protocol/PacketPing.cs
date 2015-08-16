using System;
using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketPing : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Ping; }
        }

        /// <summary>
        /// Gibt die Zeit zurück, an dem das Paket abgesendet wurde.
        /// </summary>
        public readonly long Time;

        public PacketPing(long time)
        {
            this.Time = time;
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(Time);
        }
    }
}