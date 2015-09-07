using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary
{
    public class FunctionChoreography : IChoreography
    {
        private Func<Config, TimeSpan, int, int, ushort> function;

        public FunctionChoreography(Func<Config, TimeSpan, int, int, ushort> function)
        {
            if (function == null)
                throw new ArgumentNullException("function");
            this.function = function;
        }

        public ushort GetHeight(Config config, TimeSpan time, int x, int y)
        {
            return function(config, time, x, y);
        }
    }
}
