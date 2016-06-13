using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary.Script
{
    public class TargetCluster : Target
    {
        public int ClusterX { get; private set; }
        public int ClusterY { get; private set; }

        public TargetCluster(int clusterX, int clusterY)
        {
            this.ClusterX = clusterX;
            this.ClusterY = clusterY;
        }

        public override IEnumerable<Stepper> EnumerateSteppers(Kugelmatik kugelmatik)
        {
            return kugelmatik.GetClusterByPosition(ClusterX, ClusterY).EnumerateSteppers();
        }
    }
}
