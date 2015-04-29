using System.Collections.Generic;

namespace qif2json.parser.Model
{
    internal struct Transaction
    {
        private readonly List<Line> lines;

        internal Transaction(List<Line> list)
        {
            this.lines = list;
        }

        internal IEnumerable<Line> Lines
        {
            get { return this.lines; }
        }

        internal void Add(Line line)
        {
            this.lines.Add(line);
        }
    }
}