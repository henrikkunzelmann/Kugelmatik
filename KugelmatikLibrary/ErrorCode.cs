using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary
{
    public enum ErrorCode
    {
        None = 0,
        TooShort = 1,
        InvalidX = 2,
        InvalidY = 3,
        InvalidMagic = 4,
        BufferOverflow = 5,
        UnkownPacket = 6,
        NotRunningBusy = 7,
        InvalidConfigValue = 8,
        InvalidHeight = 9,
    }
}
