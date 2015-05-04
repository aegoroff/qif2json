// Created by: egr
// Created at: 01.05.2015
// © 2015 Alexander Egorov

using System.Collections.Generic;

namespace qif2json.parser.Model
{
    public struct Transaction
    {
        private readonly List<IDictionary<string, string>> lines;

        internal Transaction(List<IDictionary<string, string>> list)
        {
            this.lines = list;
        }

        internal static Transaction Create()
        {
            return new Transaction(new List<IDictionary<string, string>>());
        }

        public IEnumerable<IDictionary<string, string>> Lines
        {
            get { return this.lines; }
        }

        internal void Add(IDictionary<string, string> line)
        {
            this.lines.Add(line);
        }
    }
}