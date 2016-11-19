using System;

namespace KugelmatikLibrary
{
    public class LogView
    {
        public LogStorage Storage { get; private set; }
        public int LastLine { get; private set; }

        public bool HasNewLines
        {
            get { return LastLine < Storage.Count; }
        }

        public LogView(LogStorage storage, int lastLine)
        {
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));
            if (lastLine < 0 || lastLine > storage.Count)
                throw new ArgumentOutOfRangeException(nameof(lastLine));

            this.Storage = storage;
            this.LastLine = lastLine;
        }

        public string[] GetNewLines()
        {
            string[] lines = Storage.GetLinesAfter(LastLine);
            LastLine += lines.Length;
            return lines;
        }
    }
}
