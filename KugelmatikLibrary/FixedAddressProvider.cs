using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary
{
    public class FixedAddressProvider : IAddressProvider
    {
        public IPAddress Address { get; private set; }

        public FixedAddressProvider(string address)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            this.Address = IPAddress.Parse(address);
        }

        public IPAddress GetAddress(Config config, int x, int y)
        {
            return Address;
        }
    }
}
