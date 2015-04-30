using System;
using qif2json.parser.Model;

namespace qif2json.parser
{
    public class Qif2JsonListener : Qif2jsonBaseListener
    {
        private Line currentLine;
        private Transaction currentTran;

        /// <summary>
        /// Occurs on transaction detection
        /// </summary>
        public event EventHandler<TransactionDetectedEventArgs> TransactionDetected;
        public event EventHandler<TypeDetectedEventArgs> TypeDetected;

        internal Qif2JsonListener()
        {
            this.JsonInstance = new Qif();
        }

        public Qif JsonInstance { get; internal set; }

        public override void ExitType(Qif2json.TypeContext context)
        {
            this.JsonInstance.Type = context.TYPE().GetText();
            this.EmitEvent();
        }

        public override void ExitAccount(Qif2json.AccountContext context)
        {
            this.JsonInstance.Type = context.ACCOUNT().GetText();
            this.EmitEvent();
        }

        private void EmitEvent()
        {
            if (this.TypeDetected != null)
            {
                this.TypeDetected(this, new TypeDetectedEventArgs(this.JsonInstance.Type));
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
            else
            {
                this.JsonInstance.Add(this.currentTran);
            }
            this.currentTran = Transaction.Create();
        }

        public override void EnterLine(Qif2json.LineContext context)
        {
            this.currentLine = new Line();
        }

        public override void ExitLine(Qif2json.LineContext context)
        {
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