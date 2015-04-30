using System;
using qif2json.parser.Model;

namespace qif2json.parser
{
    /// <summary>
    /// Transaction detection results
    /// </summary>
    public sealed class TransactionDetectedEventArgs : EventArgs
    {
        private readonly Transaction transaction;

        /// <summary>
        /// Initializes new args instance
        /// </summary>
        /// <param name="transaction">Transaction detected</param>
        public TransactionDetectedEventArgs(Transaction transaction)
        {
            this.transaction = transaction;
        }

        public Transaction Transaction
        {
            get { return this.transaction; }
        }
    }
    
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