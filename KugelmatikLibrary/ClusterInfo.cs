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
        /// Gibt den freien Speicher auf dem Cluster zurück.
        /// </summary>
        [Category("\tDebug")]
        public int FreeRam { get; private set; }

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
        /// Zeit in Sekunden wie lange schon das Cluster lief
        /// </summary>
        [Category("Time")]
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
    }
}
