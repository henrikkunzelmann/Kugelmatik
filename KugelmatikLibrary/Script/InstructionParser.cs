using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary.Script
{
    public static class InstructionParser
    { 
        public static Instruction Parse(Instruction last, string codeLine)
        {
            if (codeLine.Length == 0 || codeLine[0] == '#' || codeLine.StartsWith("//"))
                return null;

            string[] code = codeLine.ToUpper().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            int time = ParseTime(last == null ? 0 : last.Timestamp, code, 0);
            Target target;
            ushort height;

            int len;

            switch(code[1])
            {
                case "MOVE":
                case "SET":
                case "=":
                    target = ParseTarget(code, 2, out len);
                    height = ParseHeight(code, 2 + len);
                    return new InstructionMove(time, target, height);
                case "ADD":
                case "+":
                    target = ParseTarget(code, 2, out len);
                    height = ParseHeight(code, 2 + len);
                    return new InstructionAdd(time, target, height);
                case "SUB":
                case "-":
                    target = ParseTarget(code, 2, out len);
                    height = ParseHeight(code, 2 + len);
                    return new InstructionSub(time, target, height);
                default:
                    throw new CompileException(CompileException.UnknownInstruction);
            }
        }

        private static string[] units = new string[] { "MS", "MILLIS", "S", "SEC", "M", "MIN" };
        private static int[] unitFactors = new int[] { 1, 1, 1000, 1000, 1000 * 60, 1000 * 60 };

        private static int ParseTime(int last, string[] code, int offset)
        {
            if (offset >= code.Length)
                throw new CompileException(CompileException.TimestampExcepted);

            int unit = 1;
            int addFactor = 0;
            string time = code[offset];

            if (time.StartsWith("+"))
            {
                addFactor = 1;
                time = time.Substring(1);
            }
            else if (time.StartsWith("-"))
            {
                addFactor = -1;
                time = time.Substring(1);
            }

            for (int i = 0; i < units.Length; i++)
            {
                if (time.EndsWith(units[i]))
                {
                    time = time.Remove(time.Length - units[i].Length);
                    unit = unitFactors[i];
                    break;
                }
            }

            int timeValue = ParseNumber(new string[] { time }, offset) * unit;
            if (addFactor == 0)
                return timeValue;

            return last + addFactor * timeValue;
        }

        private static Target ParseTarget(string[] code, int offset, out int len)
        {
            if (code[offset] == "A" || code[offset] == "ALL")
            {
                len = 1;
                return new TargetAll();
            }

            int x, y;
            if (code[offset] == "C" || code[offset] == "CLUSTER")
            {
                x = ParseNumber(code, offset + 1);
                y = ParseNumber(code, offset + 2);
                len = 3;
                return new TargetCluster(x, y);
            }

            if (code[offset] == "S" || code[offset] == "STEPPER")
                offset++;

            x = ParseNumber(code, offset);
            y = ParseNumber(code, offset + 1);
            len = 3;
            return new TargetStepper(x, y);
        }

        private static ushort ParseHeight(string[] code, int offset)
        {
            if (offset >= code.Length)
                throw new CompileException(CompileException.HeightExcepted);
            if (code[offset] == "TOP")
                return 0;

            int value = ParseNumber(code, offset);
            if (value < 0)
                throw new CompileException(CompileException.HeightExcepted);
            return (ushort)value;
        }

        private static int ParseNumber(string[] code, int offset)
        {
            int value;
            if (offset >= code.Length || !int.TryParse(code[offset].Trim(','), out value)) // Trim, damit "MOVE X, Y, HEIGHT" moeglich ist
                throw new CompileException(CompileException.NumberExcepted);
            return value;
        }
    }
}
