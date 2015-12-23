// Created by: egr
// Created at: 01.05.2015
// © 2015 Alexander Egorov

using System;

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

        public Transaction Transaction => this.transaction;
    }
}