using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary
{
    public class BinaryStringHelper
    {
        private BinaryStringHelper()
        {

        }

        public static void WriteString(BinaryWriter writer, string str)
        {
            writer.Write((ushort)str.Length);
            for (int i = 0; i < str.Length; i++)
                writer.Write((byte)str[i]);
        }

        public static string ReadString(BinaryReader reader)
        {
            ushort len = reader.ReadUInt16();
            char[] str = new char[len];
            for (int i = 0; i < len; i++)
                str[i] = (char)reader.ReadByte();
            return new string(str);
        }
    }
}
