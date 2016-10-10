using System;
using System.IO;

namespace KugelmatikLibrary.Protocol
{
    public struct StepperPosition
    {
        public readonly byte X;
        public readonly byte Y;

        public byte Value
        {
            get
            {
                return (byte)(X << 4 | Y);
            }
        }

        public StepperPosition(byte x, byte y)
        {
            if (x > 16)
                throw new ArgumentOutOfRangeException("x");
            if (y > 16)
                throw new ArgumentOutOfRangeException("y");

            this.X = x;
            this.Y = y;
        }

        public StepperPosition(Stepper stepper)
            : this(stepper.X, stepper.Y)
        {

        }

        public StepperPosition(byte val)
        {
            this.X = (byte)((val >> 4) & 16);
            this.Y = (byte)(val & 16);
        }

        public StepperPosition(BinaryReader reader)
            : this(reader.ReadByte())
        {

        }
    }
}
