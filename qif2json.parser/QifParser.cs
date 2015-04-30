using System;
using System.IO;
using Antlr4.Runtime;
using Newtonsoft.Json;
using qif2json.parser.Model;

namespace qif2json.parser
{
    public class QifParser : IDisposable
    {
        private readonly Action<string> output;

        public QifParser(Action<string> output = null)
        {
            this.output = output;
        }

        public int NumberOfSyntaxErrors { get; private set; }
        
        public bool Idented { get; set; }

        public string Compile(string qif)
        {
            ICharStream inputStream = new AntlrInputStream(qif);
            var json = this.CompileStream(inputStream, false);
            return SerializeObject(json);
        }

        private string SerializeObject(object json)
        {
            return JsonConvert.SerializeObject(json, Idented ? Formatting.Indented : Formatting.None);
        }

        private StreamWriter outputWriter;
        
        public void CompileFile(string inputFile, string outputFile)
        {
            if (!File.Exists(inputFile))
            {
                return;
            }
            var input = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            var outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.Read);
            this.outputWriter = new StreamWriter(outputStream);
            using (outputStream)
            {
                using (input)
                {
                    ICharStream inputStream = new AntlrInputStream(input);
                    this.CompileStream(inputStream, true);
                }
                this.outputWriter.Write("]}");
                this.outputWriter.Flush();
                outputStream.Flush();
            }
        }

        internal Qif CompileStream(ICharStream inputStream, bool subscribe)
        {
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

            var listener = new Qif2JsonListener();
            if (subscribe)
            {
                listener.TransactionDetected += this.OnTransactionDetected;
                listener.TypeDetected += this.OnTypeDetected;
            }
            parser.AddParseListener(listener);
            parser.compileUnit();
            return listener.JsonInstance;
        }

        private void OnTypeDetected(object sender, TypeDetectedEventArgs e)
        {
            Write("{");
            var str = string.Format("\"Type\": \"{0}\", \"Transactions\": [", e.Type);
            Write(str);
        }

        void OnTransactionDetected(object sender, TransactionDetectedEventArgs e)
        {
            Write(SerializeObject(e.Transaction));
        }

        private void Write(string data)
        {
            if (this.outputWriter != null)
            {
                this.outputWriter.Write(data);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.outputWriter != null)
                {
                    this.outputWriter.Dispose();
                }
            }
        }
    }
}