using System;

namespace KugelmatikLibrary
{
    public class LogFlushEventArgs : EventArgs
    {
        public string Buffer { get; private set; }

        public LogFlushEventArgs(string buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            this.Buffer = buffer;
        }
    }
}
