using System.Net;

namespace KugelmatikLibrary
{
    /// <summary>
    /// Stellt Methoden bereit um für jedes Cluster eine IPAddresse zu setzen.
    /// </summary>
    public interface IAddressProvider
    {
        /// <summary>
        /// Gibt eine passende IPAddresse für ein Cluster zurück.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        IPAddress GetAddress(Config config, int x, int y);
    }
}
