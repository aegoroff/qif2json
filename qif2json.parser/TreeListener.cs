// Created by: egr
// Created at: 30.04.2015
// © 2015 Alexander Egorov

using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace qif2json.parser
{
    public class TreeListener : IParseTreeListener
    {
        private readonly Stack<string> idents = new Stack<string>();
        private readonly Action<string> output;
        private readonly Parser parser;

        public TreeListener(Action<string> output, Parser parser)
        {
            this.output = output;
            this.parser = parser;
        }

        public string Ident
        {
            get
            {
                var ident = string.Join(string.Empty, this.idents.ToArray());
                return ident;
            }
        }

        public void VisitTerminal(ITerminalNode node)
        {
            this.output(this.Ident + "  T: -> " + node.ToStringTree(this.parser));
        }

        public void VisitErrorNode(IErrorNode node)
        {
            this.output(this.Ident + "     E: -> " + node.ToStringTree(this.parser));
        }

        public void EnterEveryRule(ParserRuleContext ctx)
        {
            this.output(this.Ident + "R: -> " + ctx.ToStringTree(this.parser));
            this.idents.Push(" ");
        }

        public void ExitEveryRule(ParserRuleContext ctx)
        {
            this.idents.Pop();
            this.output(this.Ident + "CR");
        }
    }
}