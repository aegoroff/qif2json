using System;
using Antlr4.Runtime;

namespace qif2json.parser
{
    public class QifParser
    {
        private readonly Action<string> output;

        public QifParser(Action<string> output = null)
        {
            this.output = output;
        }

        public int NumberOfSyntaxErrors { get; private set; }

        public string Compile(string qif)
        {
            ICharStream inputStream = new AntlrInputStream(qif);
            var lexer = new Qif2jsonLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Qif2json(tokenStream);

#if DEBUG
            parser.Trace = true;
            if (output != null)
            {
                var treeL = new TreeListener(output, parser);
                parser.AddParseListener(treeL);
                parser.AddErrorListener(new ErrorListener(output, treeL));
            }
#endif
            NumberOfSyntaxErrors = parser.NumberOfSyntaxErrors;
            if (NumberOfSyntaxErrors > 0)
            {
                return null;
            }

            var tree = parser.compileUnit();
            var grokVisitor = new Qif2JsonVisitor();
            grokVisitor.Visit(tree);
            return string.Empty;
        }
    }
}