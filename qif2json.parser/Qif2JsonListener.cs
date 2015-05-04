// Created by: egr
// Created at: 30.04.2015
// © 2015 Alexander Egorov

using System;
using System.Collections.Generic;
using qif2json.parser.Model;

namespace qif2json.parser
{
    internal class Qif2JsonListener : Qif2jsonBaseListener
    {
        private readonly Func<string, string> keyResolver;
        private readonly Statistic fileStatistic = new Statistic();
        private IDictionary<string, string> currentLine;
        private Transaction currentTran;

        internal Qif2JsonListener(Func<string, string> keyResolver)
        {
            this.keyResolver = keyResolver;
        }

        public Statistic FileStatistic
        {
            get { return this.fileStatistic; }
        }

        /// <summary>
        ///     Occurs on transaction detection
        /// </summary>
        public event EventHandler<TransactionDetectedEventArgs> TransactionDetected;

        public event EventHandler<SyntaxElementEventArgs> TypeDetected;

        public override void EnterAccount(Qif2json.AccountContext context)
        {
            this.fileStatistic.TotalAccounts++;
        }

        public override void ExitType(Qif2json.TypeContext context)
        {
            this.EmitEvent(context.TYPE().GetText());
        }

        public override void ExitAccountType(Qif2json.AccountTypeContext context)
        {
            this.EmitEvent(context.ACCOUNT().GetText());
        }

        private void EmitEvent(string type)
        {
            this.TypeDetected.Do(handler => handler(this, new SyntaxElementEventArgs(type)));
        }

        public override void ExitHeader(Qif2json.HeaderContext context)
        {
            this.currentTran = Transaction.Create();
        }

        public override void EnterEndTransaction(Qif2json.EndTransactionContext context)
        {
            this.TransactionDetected.Do(handler => handler(this, new TransactionDetectedEventArgs(this.currentTran)));
            this.fileStatistic.TotalTransactions++;
            this.currentTran = Transaction.Create();
        }

        public override void ExitLine(Qif2json.LineContext context)
        {
            this.fileStatistic.TotalLines++;
            this.currentTran.Add(this.currentLine);
        }

        private string lineKey;

        public override void ExitCode(Qif2json.CodeContext context)
        {
            this.lineKey = keyResolver(context.LINE_START().GetText());
        }

        public override void ExitLiteral_string(Qif2json.Literal_stringContext context)
        {
            var value = context.LITERAL().GetText().TrimEnd('\r', '\n');
            this.currentLine = new Dictionary<string, string> { { this.lineKey, value } };
        }
    }
}