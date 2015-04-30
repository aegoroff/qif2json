using System;

namespace qif2json.parser.Model
{
    /// <summary>
    /// File type detection results
    /// </summary>
    public sealed class TypeDetectedEventArgs : EventArgs
    {
        private readonly string type;

        /// <summary>
        /// Initializes new args instance
        /// </summary>
        /// <param name="type">Encoding detected</param>
        public TypeDetectedEventArgs(string type)
        {
            this.type = type;
        }

        public string Type
        {
            get { return this.type; }
        }
    }
}