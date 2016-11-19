using System;
using System.Collections.Generic;
using System.Linq;

namespace KugelmatikLibrary
{
    public class LogStorage
    {
        private List<string> lines = new List<string>();
        
        /// <summary>
        /// Gibt die Anzahl aller Zeilen zurück.
        /// </summary>
        public int Count
        {
            get
            {
                lock (lines)
                {
                    return lines.Count;
                }
            }
        }

        /// <summary>
        /// Gibt alle Zeilen zurück.
        /// </summary>
        public string[] Lines
        {
            get { return lines.ToArray(); }
        }

        public event EventHandler OnAddedLines;

        /// <summary>
        /// Fügt eine Zeile zum Log hinzu.
        /// </summary>
        /// <param name="lines"></param>
        internal void AddLine(string line)
        {
            lock (lines)
            {
                lines.Add(line); 
            }

            if (OnAddedLines != null)
                OnAddedLines(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gibt alle Zeilen welche nach dem Index kommen zurück.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Die Zeilen nach dem Index oder null wenn keine neuen Zeilen vorhanden sind.</returns>
        public string[] GetLinesAfter(int index)
        {
            lock (lines)
            {
                if (index < 0 || index > Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return lines.Skip(index).ToArray();
            }
        }

        public LogView CreateView()
        {
            return CreateView(false);
        }

        public LogView CreateView(bool onlyNewData)
        {
            if (onlyNewData)
                return new LogView(this, Count);
            return new LogView(this, 0);
        }
    }
}
