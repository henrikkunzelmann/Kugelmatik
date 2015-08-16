using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary
{
    public class KugelmatikAddressProvider : IAddressProvider
    {
        public IPAddress GetAddress(Config config, int x, int y)
        {
            byte lanID = (byte)((y + 1) * 10 + (x + 1));
            return new IPAddress(new byte[] { 192, 168, 88, lanID });
        }
    }
}
