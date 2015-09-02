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

        public ClusterConfig Config;

        public PacketConfig(ClusterConfig config)
        {
            this.Config = config;
        }

        public void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            byte stepMode = reader.ReadByte();

            if (!Enum.IsDefined(typeof(StepMode), stepMode))
                throw new InvalidDataException("Unkown step mode: " + stepMode);

            Config = new ClusterConfig((StepMode)stepMode,
                reader.ReadInt32(),
                reader.ReadByte() > 0);
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write((byte)Config.StepMode);
            writer.Write(Config.TickTime);
            writer.Write((byte)(Config.UseBreak ? 1 : 0));
        }
    }
}