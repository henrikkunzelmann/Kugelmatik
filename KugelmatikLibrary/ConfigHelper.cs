using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace KugelmatikLibrary
{
    public class ConfigHelper
    {
        private ConfigHelper()
        {

        }

        private static Type[] numericTypes = new Type[] { typeof(byte), typeof(sbyte), typeof(ushort), typeof(short), typeof(uint), typeof(int), typeof(ulong), typeof(long) };

        public static void SaveToFile<T>(string file, T config)
        {
            using (FileStream stream = File.Open(file, FileMode.Create))
            {
                StreamWriter writer = new StreamWriter(stream);

                writer.WriteLine("# Standard Einstellungen für die Kugelmatik V3");
                writer.WriteLine();

                foreach (FieldInfo info in typeof(T).GetFields())
                {
                    DescriptionAttribute help = info.GetCustomAttribute<DescriptionAttribute>();
                    if (help != null)
                        writer.WriteLine("# {0}", help.Description);
                    Range range = info.GetCustomAttribute<Range>();
                    if (range != null)
                    {
                        if (range.Min == int.MinValue)
                            writer.WriteLine("# Bereich: <= {0}", range.Max);
                        else if (range.Max == int.MaxValue)
                            writer.WriteLine("# Bereich: >= {0}", range.Min);
                        else
                            writer.WriteLine("# Bereich: {0} - {1}", range.Min, range.Max);
                    }

                    writer.Write(info.Name);
                    writer.Write(" = ");
                    writer.WriteLine(info.GetValue(config));
                    writer.WriteLine();
                }

                writer.Flush();
            }
        }

        /// <summary>
        /// Lädt die Einstellungen aus einer Textdatei.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static T LoadConfigFromFile<T>(string file, T config)
        {
            string[] fileLines = File.ReadAllLines(file);
            Type configType = typeof(T);

            object data = config;
            for (int i = 0; i < fileLines.Length; i++)
            {
                string line = fileLines[i].Trim();
                if (line.Length == 0 || line.StartsWith("#")) // # wird für Kommentare benutzt
                    continue;

                string[] keyValue = line.Split('=');
                if (keyValue.Length == 0)
                    throw new InvalidDataException(string.Format("Unexpected line {0} in config file: '=' was not found.", i + 1));

                // Feld in der Config-Klasse finden
                FieldInfo field = configType.GetField(keyValue[0].Trim(), BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (field == null) // wenn Feld nicht gefunden dann ignorieren
                    continue;

                if (field.FieldType == typeof(string))
                    field.SetValue(data, keyValue[1].Trim());
                else if (numericTypes.Contains(field.FieldType))
                {
                    long value;
                    if (!TryParseLong(keyValue[1].Trim(), out value))
                        throw new InvalidDataException(string.Format("Unexpected line {0} in config file: can not parse number.", i + 1));

                    Range range = field.GetCustomAttribute<Range>();
                    if (range != null)
                    {
                        if (value < range.Min)
                            throw new InvalidDataException(string.Format("Unexpected line {0} in config file: value is smaller then {1}.", i + 1, range.Min));
                        else if (value > range.Max)
                            throw new InvalidDataException(string.Format("Unexpected line {0} in config file: value is bigger then {1}.", i + 1, range.Max));
                    }
                    field.SetValue(data, Convert.ChangeType(value, field.FieldType));
                }
                else if (field.FieldType == typeof(bool))
                {
                    string value = keyValue[1].Trim().ToLower();
                    if (value == "true" || value == "yes")
                        field.SetValue(data, true);
                    else if (value == "false" || value == "no")
                        field.SetValue(data, false);
                    else
                        throw new InvalidDataException(string.Format("Unexpected line {0} in config file: can not parse boolean value.", i + 1));
                }
                else if (field.FieldType.IsEnum)
                {
                    string value = keyValue[1].Trim().ToLower();
                    field.SetValue(data, Enum.Parse(field.FieldType, value, true));
                }
                else
                    throw new NotImplementedException();
            }

            return (T)data;
        }

        private static bool TryParseLong(string s, out long result)
        {
            if (s.StartsWith("0x"))
                return long.TryParse(s.Remove(0, "0x".Length), NumberStyles.AllowHexSpecifier, null, out result);
            return long.TryParse(s, out result);
        }
    }
}
