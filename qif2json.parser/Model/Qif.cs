// Created by: egr
// Created at: 30.04.2015
// © 2015 Alexander Egorov

using System.Collections.Generic;

namespace qif2json.parser.Model
{
    public class Qif
    {
        private readonly List<Transaction> transactions;

        internal Qif()
        {
            this.transactions = new List<Transaction>();
        }

        public string Type { get; set; }

        public IEnumerable<Transaction> Transactions
        {
            get { return this.transactions; }
        }

        internal void Add(Transaction transaction)
        {
            this.transactions.Add(transaction);
        }
    }
}