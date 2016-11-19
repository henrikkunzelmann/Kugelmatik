using System;
using System.ComponentModel;

namespace KugelmatikLibrary
{
    /// <summary>
    /// Stellt verschiedene Informationen über das Cluster bereit.
    /// </summary>
    public class ClusterInfo : IEquatable<ClusterInfo>
    {
        /// <summary>
        /// Gibt die Build-Version der Firmware des Clusters zurück.
        /// </summary>
        public byte BuildVersion { get; private set; }

        /// <summary>
        /// Gibt true zurück, wenn das Cluster einen Busy-Befehl ausführt.
        /// </summary>
        public BusyCommand CurrentBusyCommand { get; private set; }

        /// <summary>
        /// Gibt die größte Revision des Clusters zurück.
        /// </summary>
        public int HighestRevision { get; private set; }

        /// <summary>
        /// Gibt die Einstellungen des Clusters zurück.
        /// </summary>
        [TypeConverter(typeof(ConfigTypeConverter))]
        public ClusterConfig Config { get; private set; }

        /// <summary>
        /// Gibt den letzten Fehler zurück der auf dem Cluster aufgetreten ist.
        /// </summary>
        public ErrorCode LastError { get; private set; }

        /// <summary>
        /// Gibt den freien Speicher auf dem Cluster zurück.
        /// </summary>
        public int FreeRam { get; private set; }

        /// <summary>
        /// Bitfeld, welches angibt welche MCP ansprechbar sind.
        /// </summary>
        public byte MCPStatus { get; private set; }

        /// <summary>
        /// Zeit die das Cluster benötigt um einen Tick auszuführen.
        /// </summary>
        public int LoopTime { get; private set; }

        /// <summary>
        /// Zeit die das Cluster benötigt um ein Paket zu verarbeiten.
        /// </summary>
        public int NetworkTime { get; private set; }

        /// <summary>
        /// Zeit die das Cluster maximal benötigt um ein Paket zu verarbeiten.
        /// </summary>
        public int MaxNetworkTime { get; private set; }

        /// <summary>
        /// Zeit die das Cluster benötigt um die Stepper zu aktualisieren.
        /// </summary>
        public int StepperTime { get; private set; }

        /// <summary>
        /// Zeit in Sekunden wie lange schon das Cluster lief
        /// </summary>
        public int Uptime { get; private set; }

        public ClusterInfo(byte buildVersion, BusyCommand currentBusyCommand, int highestRevision, 
            ClusterConfig config, ErrorCode lastError, int freeRam, 
            byte mcpStatus, int loopTime, int networkTime, int maxNetworkTime, int stepperTimer,
            int upTime)
        {
            this.BuildVersion = buildVersion;
            this.CurrentBusyCommand = currentBusyCommand;
            this.HighestRevision = highestRevision;
            this.Config = config;
            this.LastError = lastError;
            this.FreeRam = freeRam;
            this.MCPStatus = mcpStatus;
            this.LoopTime = loopTime;
            this.NetworkTime = networkTime;
            this.MaxNetworkTime = maxNetworkTime;
            this.StepperTime = stepperTimer;
            this.Uptime = upTime;
        }

        public static bool operator ==(ClusterInfo a, ClusterInfo b)
        {
            if (object.ReferenceEquals(a, b))
                return true;
            if (object.ReferenceEquals(a, null))
                return false;
            return a.Equals(b);
        }

        public static bool operator !=(ClusterInfo a, ClusterInfo b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is ClusterInfo)
                return Equals(obj as ClusterInfo);
            return false;
        }

        public bool Equals(ClusterInfo other)
        {
            if (other == null)
                return false;

            return BuildVersion == other.BuildVersion
                && CurrentBusyCommand == other.CurrentBusyCommand
                && HighestRevision == other.HighestRevision
                && Config.Equals(other.Config)
                && LastError == other.LastError
                && FreeRam == other.FreeRam
                && MCPStatus == other.MCPStatus;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = hash * 7 + BuildVersion.GetHashCode();
                hash = hash * 7 + CurrentBusyCommand.GetHashCode();
                hash = hash * 7 + HighestRevision.GetHashCode();
                hash = hash * 7 + Config.GetHashCode();
                hash = hash * 7 + LastError.GetHashCode();
                hash = hash * 7 + FreeRam.GetHashCode();
                hash = hash * 7 + MCPStatus.GetHashCode();
                return hash;
            }
        }
    }
}
