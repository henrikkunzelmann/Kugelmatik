using System;
using System.Reflection;
using System.ComponentModel;

namespace KugelmatikLibrary
{
    /// <summary>
    /// Stellt verschiedene Einstellungen für die Kugelmatik bereit.
    /// </summary>
    [TypeConverter(typeof(ConfigTypeConverter))]
    public struct Config
    {
        [Category("Kugelmatik")]
        [DescriptionAttribute("Anzahl der Cluster der Kugelmatik in der Breite (X)")]
        [Range(1, int.MaxValue)]
        public int KugelmatikWidth;

        [Category("Kugelmatik")]
        [DescriptionAttribute("Anzahl der Cluster der Kugelmatik in der Höhe (Y)")]
        [Range(1, int.MaxValue)]
        public int KugelmatikHeight;

        [Category("Network Log")]
        [DescriptionAttribute("Wenn true, dann werden gesendete Pakete in die Konsole geschrieben")]
        public bool VerbosePacketSending;

        [Category("Network Log")]
        [DescriptionAttribute("Wenn true, dann werden empfangene Pakete in die Konsole geschrieben")]
        public bool VerbosePacketReceive;

        [Category("Network Log")]
        [DescriptionAttribute("Wenn true, dann werden Ping-Pakete in die Konsole geschrieben")]
        public bool LogPingPacket;

        [Category("Network")]
        [DescriptionAttribute("Setzt eine IP-Adresse die für ein Cluster benutzt werden soll.")]
        public string FixedIP;

        [Category("Network")]
        [DescriptionAttribute("Setzt eine Datei die für die Zuordnung der IP-Adressen benutzt wird")]
        public string AddressFile;

        [Category("Network")]
        [DescriptionAttribute("Port der UDP-Verbindung zu den Clustern")]
        [Range(0, 65535)]
        public int ProtocolPort;

        [Category("Network")]
        [DescriptionAttribute("Die Zeitspanne vom Abschicken eines Pakets bis das Paket neugeschickt wird")]
        [Range(10, int.MaxValue)]
        public int AcknowlegdeTime;

        [Category("Network")]
        [DescriptionAttribute("Standard Wartezeit die jeder Schrittmotor zwischen jedem Schritt warten soll")]
        [Range(0, byte.MaxValue)]
        public int WaitTime;

        [Category("Network")]
        [DescriptionAttribute("Wenn true, dann werden Pakete (außer Ping) nicht gesendet, wenn das Cluster offline ist.")]
        public bool IgnorePacketsWhenOffline;

        [Category("Network")]
        [DescriptionAttribute("Wenn true, dann wird der Guaranteed-Flag ignoriert, wenn das Cluster offline ist.")]
        public bool IgnoreGuaranteedWhenOffline;

        [Category("Network")]
        [DescriptionAttribute("Wenn true, dann wird der Guaranteed-Flag ignoriert.")]
        public bool IgnoreGuaranteed;

        public static Config GetDefault()
        {
            Config config = new Config();
            config.ProtocolPort = 14804;
            config.KugelmatikWidth = 1;
            config.KugelmatikHeight = 1;
            config.VerbosePacketSending = true;
            config.VerbosePacketReceive = true;
            config.FixedIP = "";
            config.AddressFile = "";
            config.AcknowlegdeTime = 20;
            config.WaitTime = 0;
            config.IgnorePacketsWhenOffline = true;
            config.IgnoreGuaranteedWhenOffline = true;
            config.IgnoreGuaranteed = false;
            return config;
        }

        public void PrintToConsole()
        {
            Console.WriteLine("Config:");
            foreach (FieldInfo info in GetType().GetFields())
                Console.WriteLine("{0} {1} = {2}", info.FieldType.Name, info.Name, info.GetValue(this));
            Console.WriteLine();
        }
    }
}
