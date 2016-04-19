using System;
using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketConfig2 : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Config2; }
        }

        public ClusterConfig Config;

        public PacketConfig2(ClusterConfig config)
        {
            this.Config = config;
        }

        public void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            Config = ClusterConfig.Read(reader);
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(ClusterConfig.Size);
            Config.Write(writer);
        }
    }
}