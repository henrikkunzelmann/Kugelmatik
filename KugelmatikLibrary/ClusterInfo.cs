using System;
using System.ComponentModel;
using System.IO;

namespace KugelmatikLibrary
{
    /// <summary>
    /// Stellt verschiedene Informationen über das Cluster bereit.
    /// </summary>
    public class ClusterInfo
    {
        /// <summary>
        /// Gibt die Build-Version der Firmware des Clusters zurück.
        /// </summary>
        [Category("\t\tInfo")]
        public byte BuildVersion { get; private set; }

        /// <summary>
        /// Gibt true zurück, wenn das Cluster einen Busy-Befehl ausführt.
        /// </summary>
        [Category("\t\tInfo")]
        public BusyCommand CurrentBusyCommand { get; private set; }

        /// <summary>
        /// Gibt die größte Revision des Clusters zurück.
        /// </summary>
        [Category("\tDebug")]
        public int HighestRevision { get; private set; }

        /// <summary>
        /// Gibt die Einstellungen des Clusters zurück.
        /// </summary>
        [TypeConverter(typeof(ConfigTypeConverter))]
        [Category("\t\tInfo")]
        public ClusterConfig Config { get; private set; }

        /// <summary>
        /// Gibt den letzten Fehler zurück der auf dem Cluster aufgetreten ist.
        /// </summary>
        [Category("\tDebug")]
        public ErrorCode LastError { get; private set; }

        /// <summary>
        /// Gibt den freien Speicher in Bytes auf dem Cluster zurück.
        /// </summary>
        [Category("\tDebug")]
        public ulong FreeRam { get; private set; }

        /// <summary>
        /// Bitfeld, welches angibt welche MCP ansprechbar sind.
        /// </summary>
        [Category("\tDebug")]
        public byte MCPStatus { get; private set; }

        /// <summary>
        /// Zeit die das Cluster benötigt um einen Tick auszuführen.
        /// </summary>
        [Category("Time")]
        public int LoopTime { get; private set; }

        /// <summary>
        /// Zeit die das Cluster benötigt um ein Paket zu verarbeiten.
        /// </summary>
        [Category("Time")]
        public int NetworkTime { get; private set; }

        /// <summary>
        /// Zeit die das Cluster maximal benötigt um ein Paket zu verarbeiten.
        /// </summary>
        [Category("Time")]
        public int MaxNetworkTime { get; private set; }

        /// <summary>
        /// Zeit die das Cluster benötigt um die Stepper zu aktualisieren.
        /// </summary>
        [Category("Time")]
        public int StepperTime { get; private set; }

        /// <summary>
        /// Zeit in Sekunden wie lange schon das Cluster lief.
        /// </summary>
        [Category("Time")]
        public int Uptime { get; private set; }

        public ClusterInfo(BinaryReader reader)
        {
            BuildVersion = reader.ReadByte();
            CurrentBusyCommand = (BusyCommand)reader.ReadByte();
            HighestRevision = reader.ReadInt32();

            int error = reader.ReadByte();
            if (!Enum.IsDefined(typeof(ErrorCode), error))
                LastError = ErrorCode.UnknownError;
            else
                LastError = (ErrorCode)error;

            if (BuildVersion < 20)
                FreeRam = (ulong)Math.Max((short)0, reader.ReadInt16());
            else
                FreeRam = reader.ReadUInt64();

            ushort configSize = reader.ReadUInt16();
            if (configSize != ClusterConfig.Size)
            {
                Config = new ClusterConfig();
                Log.Error("Packet config size does not match config structure size (version: {0}, size (firmware != local): {1} != {2})", BuildVersion, configSize, ClusterConfig.Size);
                reader.BaseStream.Seek(configSize, SeekOrigin.Current);
            }
            else
                Config = ClusterConfig.Read(reader);

            MCPStatus = reader.ReadByte();
            LoopTime = reader.ReadInt32();
            NetworkTime = reader.ReadInt32();
            MaxNetworkTime = reader.ReadInt32();
            StepperTime = reader.ReadInt32();
            Uptime = reader.ReadInt32();
        }
    }
}
