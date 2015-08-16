using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary
{
    public interface IChoreography
    {
        ushort GetHeight(Config config, TimeSpan time, int x, int y);
    }
}
