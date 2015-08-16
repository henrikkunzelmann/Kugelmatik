using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary.Protocol
{
    public static class PacketTypeHelper
    {
        /// <summary>
        /// Gibt zurück, ob das Cluster bei einem Paket-Typ antwortet.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool DoesClusterAnswer(this PacketType type)
        {
            switch(type)
            {
                case PacketType.Ping:
                case PacketType.Info:
                case PacketType.GetData:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gibt zurück, ob der Paket-Typ ein busy-Befehl ist.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsBusyCommand(this PacketType type)
        {
            switch(type)
            {
                case PacketType.Home:
                case PacketType.Fix:
                case PacketType.HomeStepper:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gibt zurück, ob das Cluster ein Paket verarbeitet wenn es gerade einen busy-Befehl ausführt.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CanSendWhileBusy(this PacketType type)
        {
            switch(type)
            {
                case PacketType.Ping:
                case PacketType.Info:
                case PacketType.Stop:
                    return true;
                default:
                    return false;
            }
        }
    }
}
