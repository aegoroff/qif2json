// Created by: egr
// Created at: 01.05.2015
// © 2015 Alexander Egorov

using System.Collections.Generic;

namespace qif2json.parser.Model
{
    public struct Transaction
    {
        private readonly List<Line> lines;

        internal Transaction(List<Line> list)
        {
            this.lines = list;
        }

        internal static Transaction Create()
        {
            return new Transaction(new List<Line>());
        }

        public IEnumerable<Line> Lines
        {
            get { return this.lines; }
        }

        internal void Add(Line line)
        {
            this.lines.Add(line);
        }
    }
}