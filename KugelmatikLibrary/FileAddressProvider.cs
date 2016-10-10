using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace KugelmatikLibrary
{
    public class FileAddressProvider : IAddressProvider
    {
        private Dictionary<string, IPAddress> addresses = new Dictionary<string, IPAddress>();
        private IAddressProvider fallback;
        
        public FileAddressProvider(string file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            string[] lines = File.ReadAllLines(file);
            foreach(string line in lines)
            {
                string lineTrimmed = line.Trim();
                if (lineTrimmed.StartsWith("#"))
                    continue;

                int equalChar = lineTrimmed.IndexOf('=');
                if (equalChar < 0)
                    continue;

                string key = lineTrimmed.Substring(0, equalChar).Trim();
                string value = lineTrimmed.Remove(0, equalChar + 1).Trim();

                if (key.ToLower() == "fallback")
                {
                    switch (value.ToLower())
                    {
                        case "kugelmatik":
                            fallback = new KugelmatikAddressProvider();
                            break;
                        default:
                            Log.Error("FileAddressProvider: unknown fallback {0}", value);
                            break;
                    }
                }
                else
                    addresses.Add(key, IPAddress.Parse(value));
            }
        }

        public IPAddress GetAddress(Config config, int x, int y)
        {
            IPAddress address;
            if (addresses.TryGetValue(string.Format("{0}_{1}", x, y), out address))
                return address;
            if (fallback != null)
                return fallback.GetAddress(config, x, y);
            return null;
        }
    }
}
