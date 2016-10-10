using System;
using System.Collections.Generic;
using System.IO;

namespace KugelmatikLibrary.Script
{
    public class KugelmatikScript : Choreography
    {
        private List<Instruction> instructions;
        private int current;

        private KugelmatikScript(List<Instruction> instructions)
        {
            this.instructions = instructions;
        }

        public static KugelmatikScript LoadScript(string file)
        {
            List<Instruction> instructions = new List<Instruction>();
            Instruction last = null;

            string[] lines = File.ReadAllLines(file);
            for (int i = 0; i < lines.Length; i++)
            {
                try
                {
                    Instruction instruction = InstructionParser.Parse(last, lines[i]);
                    if (instruction == null)
                        continue;

                    instructions.Add(instruction);
                    last = instruction;
                }
                catch (CompileException e)
                {
                    throw new CompileException(i + 1, e.Message);
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                    throw new CompileException(i + 1, CompileException.InternalError);
                }
            }

            return new KugelmatikScript(instructions);
        }

        public override void Tick(Kugelmatik kugelmatik, TimeSpan time)
        {
            while (true)
            {
                if (current >= instructions.Count)
                    return;

                Instruction instruction = instructions[current];
                if (time.TotalMilliseconds < instruction.Timestamp)
                    return;

                instruction.Execute(kugelmatik);
                current++;
            }
        }
    }
}
