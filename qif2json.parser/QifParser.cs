// Created by: egr
// Created at: 30.04.2015
// © 2015 Alexander Egorov

using System;
using System.IO;
using Antlr4.Runtime;
using Newtonsoft.Json;
using qif2json.parser.Model;

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
        
        public bool Idented { get; set; }


        public string CompileString(string qifString)
        {
            var model = new Qif();
            ICharStream inputStream = new AntlrInputStream(qifString);
            this.CompileStream(
                inputStream, 
                (o, args) => model.Type = args.Type, 
                (o, args) => model.Add(args.Transaction)
                );
            return SerializeObject(model);
        }
        
        public void CompileFile(string inputFile, string outputFile)
        {
            if (!File.Exists(inputFile))
            {
                return;
            }
            var input = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            var outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.Read);
            var outputWriter = new StreamWriter(outputStream);
            var started = false;
            using (outputStream)
            {
                using (outputWriter)
                {
                    using (input)
                    {
                        ICharStream inputStream = new AntlrInputStream(input);
                        this.CompileStream(inputStream,
                            (o, args) => outputWriter.Write(CreateHead(args.Type)),
                            (o, args) =>
                            {
                                if (started)
                                {
                                    // write comma only after first transaction
                                    outputWriter.Write(",");
                                }
                                started = true;
                                outputWriter.Write(this.SerializeObject(args.Transaction));
                            });
                    }
                    outputWriter.Write("]}");
                    outputWriter.Flush();
                    outputStream.Flush();
                }
            }
        }

        private void CompileStream(
            ICharStream inputStream, 
            Action<object, TypeDetectedEventArgs> onTypeDetect, 
            Action<object, TransactionDetectedEventArgs> onTransactionDetect)
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
                return;
            }

            var listener = new Qif2JsonListener();
            listener.TransactionDetected += (sender, e) => onTransactionDetect(sender, e);
            listener.TypeDetected += (sender, e) => onTypeDetect(sender, e);
            parser.AddParseListener(listener);
            parser.compileUnit();
        }

        private string SerializeObject(object json)
        {
            return JsonConvert.SerializeObject(json, this.Idented ? Formatting.Indented : Formatting.None);
        }

        private static string CreateHead(string type)
        {
            return "{" + string.Format("\"Type\": \"{0}\", \"Transactions\": [", type);
        }
    }
}