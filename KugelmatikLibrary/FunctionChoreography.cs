using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary
{
    public class FunctionChoreography : IChoreography
    {
        private Func<Cluster, TimeSpan, int, int, ushort> function;

        public FunctionChoreography(Func<Cluster, TimeSpan, int, int, ushort> function)
        {
            if (function == null)
                throw new ArgumentNullException("function");
            this.function = function;
        }

        public ushort GetHeight(Cluster cluster, TimeSpan time, int x, int y)
        {
            return function(cluster, time, x, y);
        }
    }
}
