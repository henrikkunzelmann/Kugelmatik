using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary
{
    public enum BusyCommand : byte
    {
        None = 0,
        Home = 1,
        Fix = 2,
        HomeStepper = 3,
        Unknown = 255
    }
}
