using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary
{
    /// <summary>
    /// Einstellungen eines Clusters.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    [TypeConverter(typeof(ConfigTypeConverter))]
    public struct ClusterConfig
    {
        [DescriptionAttribute("Schrittmodus")]
        [MarshalAs(UnmanagedType.U1)]
        public StepMode StepMode;

        [DescriptionAttribute("Bremsmodus")]
        [MarshalAs(UnmanagedType.U1)]
        public BrakeMode BrakeMode;

        [DescriptionAttribute("Zeit (in Mikrosekunden) zwischen zwei Schritten")]
        [Range(200, 10000)]
        public uint TickTime;

        [Category("Kugelmatik")]
        [DescriptionAttribute("Zeit (in Mikrosekunden) zwischen zwei Schritten (beim Home-Befehl)")]
        [Range(200, 10000)]
        public uint HomeTime;

        [Category("Kugelmatik")]
        [DescriptionAttribute("Zeit (in Mikrosekunden) zwischen zwei Schritten (beim Fix-Befehl)")]
        [Range(200, 10000)]
        public uint FixTime;

        [Category("Kugelmatik")]
        [DescriptionAttribute("Anzahl der Schritte die eine Kugel maximal nach unten darf")]
        [Range(1, 8000)]
        public short MaxSteps;

        [Category("Kugelmatik")]
        [DescriptionAttribute("Anzahl der Schritte für den Home-Befehl")]
        [Range(1, 8000)]
        public short HomeSteps;

        [Category("Kugelmatik")]
        [DescriptionAttribute("Anzahl der Schritte für den Fix-Befehl")]
        [Range(1, 10000)]
        public short FixSteps;

        [Category("Kugelmatik")]
        [DescriptionAttribute("Zeit (in Ticks) wie lange die Bremse gehalten wird (für SmartBrake)")]
        [Range(0, ushort.MaxValue)]
        public ushort BrakeTicks;

        public static ushort Size
        {
            get { return (ushort)Marshal.SizeOf(typeof(ClusterConfig)); }
        }

        public static ClusterConfig GetDefault()
        {
            ClusterConfig config = new ClusterConfig();
            config.StepMode = StepMode.Full;
            config.BrakeMode = BrakeMode.Smart;
            config.TickTime = 1900;
            config.HomeTime = 3000;
            config.FixTime = 3000;
            
            config.MaxSteps = 8000;
            config.HomeSteps = 8000;
            config.FixSteps = 8000;
            
            config.BrakeTicks = 10000;
            return config;
        }

        public static ClusterConfig GetCompatibility(StepMode stepMode, int delayTime, bool useBrake)
        {
            if (delayTime < 200 || delayTime > 10000)
                throw new ArgumentOutOfRangeException("delayTime");

            ClusterConfig config = GetDefault();
            config.StepMode = stepMode;
            config.TickTime = (uint)delayTime;
            config.BrakeMode = useBrake ? BrakeMode.Always : BrakeMode.None;
            return config;
        }

        public static ClusterConfig Read(BinaryReader reader)
        {
            byte[] buffer = new byte[Size];
            reader.Read(buffer, 0, buffer.Length);

            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            ClusterConfig settings = (ClusterConfig)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(ClusterConfig));
            handle.Free();

            return settings;
        }

        public void Write(BinaryWriter writer)
        {
            byte[] buffer = new byte[Size];

            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(this, handle.AddrOfPinnedObject(), false);
            handle.Free();

            writer.Write(buffer, 0, buffer.Length);
        }
    }
}
