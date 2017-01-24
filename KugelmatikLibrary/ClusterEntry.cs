using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace KugelmatikLibrary
{
    public class ClusterEntry
    {
        public IPAddress Address { get; private set; }
        public byte FirmwareVersion { get; private set; }

        public ClusterEntry(IPAddress address, byte firmwareVersion)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            this.Address = address;
            this.FirmwareVersion = firmwareVersion;
        }

        public override string ToString()
        {
            return string.Format("{0} (version: {1})", Address, FirmwareVersion);
        }
    }
}
