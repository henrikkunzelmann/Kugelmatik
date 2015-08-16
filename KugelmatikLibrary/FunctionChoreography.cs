using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary
{
    public class FunctionChoreography
    {
        private Func<TimeSpan, int, int, int> function;

        public FunctionChoreography(Func<TimeSpan, int, int,  int> function)
        {
            if (function == null)
                throw new ArgumentNullException("function");
            this.function = function;
        }

        public int GetHeight(TimeSpan time, int x, int y)
        {
            return function(time, x, y);
        }
    }
}
