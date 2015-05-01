// Created by: egr
// Created at: 30.04.2015
// © 2015 Alexander Egorov

using System;
using qif2json.parser.Model;

namespace qif2json.parser
{
    internal class Qif2JsonListener : Qif2jsonBaseListener
    {
        private Line currentLine;
        private Transaction currentTran;

        /// <summary>
        /// Occurs on transaction detection
        /// </summary>
        public event EventHandler<TransactionDetectedEventArgs> TransactionDetected;
        public event EventHandler<TypeDetectedEventArgs> TypeDetected;

        private readonly Statistic fileStatistic = new Statistic();

        public Statistic FileStatistic
        {
            get { return this.fileStatistic; }
        }

        public override void ExitType(Qif2json.TypeContext context)
        {
            this.EmitEvent(context.TYPE().GetText());
        }

        public override void ExitAccount(Qif2json.AccountContext context)
        {
            this.EmitEvent(context.ACCOUNT().GetText());
        }

        private void EmitEvent(string type)
        {
            this.fileStatistic.FileType = type;
            if (this.TypeDetected != null)
            {
                this.TypeDetected(this, new TypeDetectedEventArgs(type));
            }
        }

        public override void ExitHeader(Qif2json.HeaderContext context)
        {
            this.currentTran = Transaction.Create();
        }

        public override void EnterEndTransaction(Qif2json.EndTransactionContext context)
        {
            if (TransactionDetected != null)
            {
                this.TransactionDetected(this, new TransactionDetectedEventArgs(this.currentTran));
            }
            this.fileStatistic.TotalTransactions++;
            this.currentTran = Transaction.Create();
        }

        public override void EnterLine(Qif2json.LineContext context)
        {
            this.currentLine = new Line();
        }

        public override void ExitLine(Qif2json.LineContext context)
        {
            this.fileStatistic.TotalLines++;
            this.currentTran.Add(this.currentLine);
        }

        public override void ExitCode(Qif2json.CodeContext context)
        {
            this.currentLine.Code = context.LINE_START().GetText();
        }

        public override void ExitLiteral_string(Qif2json.Literal_stringContext context)
        {
            this.currentLine.Value = context.LITERAL().GetText().TrimEnd('\r', '\n');
        }
    }
}