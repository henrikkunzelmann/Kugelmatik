using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary
{
    public class ClusterConfig : IEquatable<ClusterConfig>
    {
        public StepMode StepMode { get; private set; }
        public int TickTime { get; private set; }
        public bool UseBreak { get; private set; }

        public ClusterConfig()
        {
            StepMode = StepMode.Half;
            TickTime = 4000;
            UseBreak = false;
        }

        public ClusterConfig(Config config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            StepMode = config.ClusterStepMode;
            TickTime = config.ClusterTickTime;
            UseBreak = config.ClusterUseBreak;
        }

        public ClusterConfig(StepMode stepMode, int delayTime, bool useBreak)
        {
            if (delayTime < 50 || delayTime > 15000)
                throw new ArgumentOutOfRangeException("delayTime");

            this.StepMode = stepMode;
            this.TickTime = delayTime;
            this.UseBreak = useBreak;
        }

        public static bool operator ==(ClusterConfig a, ClusterConfig b)
        {
            if (object.ReferenceEquals(a, b))
                return true;
            if (object.ReferenceEquals(a, null))
                return false;
            return a.Equals(b);
        }

        public static bool operator !=(ClusterConfig a, ClusterConfig b)
        {
            return !(a == b);
        }


        public override bool Equals(object obj)
        {
            if (obj is ClusterConfig)
                return Equals(obj as ClusterConfig);
            return false;
        }

        public bool Equals(ClusterConfig other)
        {
            if (other == null)
                return false;
            return StepMode == other.StepMode 
                && TickTime == other.TickTime 
                && UseBreak == other.UseBreak;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + StepMode.GetHashCode();
                hash = (hash * 7) + TickTime.GetHashCode();
                hash = (hash * 7) + UseBreak.GetHashCode();
                return hash;
            }
        }
    }
}
