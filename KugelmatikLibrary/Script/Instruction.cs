using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary.Script
{
    public abstract class Instruction
    {
        public int Timestamp { get; private set; }

        public Instruction(int timestamp)
        {
            this.Timestamp = timestamp;
        }

        public abstract void Execute(Kugelmatik kugelmatik);
    }
}
