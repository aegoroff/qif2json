// Created by: egr
// Created at: 30.04.2015
// © 2015 Alexander Egorov

using System;
using Antlr4.Runtime;

namespace qif2json.parser.Debug
{
    public class ErrorListener : IAntlrErrorListener<IToken>
    {
        private readonly Action<string> output;
        private readonly TreeListener treeListener;

        public ErrorListener(Action<string> output, TreeListener treeListener)
        {
            this.output = output;
            this.treeListener = treeListener;
        }

        public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine,
            string msg,
            RecognitionException e)
        {
            this.output(this.treeListener.Ident + "    " + msg);
        }
    }
}