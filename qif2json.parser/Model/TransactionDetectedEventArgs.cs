﻿using System;

namespace qif2json.parser.Model
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
}