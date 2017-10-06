using System;
using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public class PacketStartOTA : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.StartOTA; }
        }

        public string File;

        public PacketStartOTA(string file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            this.File = file;
        }
        
        public void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            File = BinaryStringHelper.ReadString(reader);
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            BinaryStringHelper.WriteString(writer, File);
        }
    }
}
