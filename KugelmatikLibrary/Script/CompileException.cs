using System;

namespace KugelmatikLibrary.Script
{
    [SerializableAttribute]
    public class CompileException : Exception
    {
        public static readonly string UnknownError = "Unknown error";
        public static readonly string InternalError = "Internal error";
        public static readonly string NumberExcepted = "Number excepted";
        public static readonly string UnknownInstruction = "Unknown instruction";
        public static readonly string TimestampExcepted = "Timestamp excepted";
        public static readonly string HeightExcepted = "Height excepted";

        public int Line { get; private set; }

        public CompileException(string message)
            : base(message)
        {

        }

        public CompileException(int line, string message)
            : base(message)
        {
            this.Line = line;
        }
    }
}
