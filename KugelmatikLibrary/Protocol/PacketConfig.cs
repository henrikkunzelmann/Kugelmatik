using System;
using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct PacketConfig : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Config; }
        }

        public readonly ClusterConfig Config;

        public PacketConfig(ClusterConfig config)
        {
            this.Config = config;
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write((byte)Config.StepMode);
            writer.Write(Config.DelayTime);
            writer.Write((byte)(Config.UseBreak ? 1 : 0));
        }
    }
}