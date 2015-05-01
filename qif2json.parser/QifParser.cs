// Created by: egr
// Created at: 30.04.2015
// © 2015 Alexander Egorov

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Newtonsoft.Json;
using qif2json.parser.Model;

#if DEBUG
using qif2json.parser.Debug;
#endif

namespace qif2json.parser
{
    public class QifParser
    {
#if DEBUG
        private readonly Action<string> output;
#endif

        public QifParser(Action<string> output = null)
        {
#if DEBUG
            this.output = output;
#endif
        }


        public int NumberOfSyntaxErrors { get; private set; }
        
        public bool Idented { get; set; }

        public Statistic FileStatistic { get; private set; }


        public string CompileString(string qifString)
        {
            var input = new MemoryStream(Encoding.Unicode.GetBytes(qifString));
            var output = new MemoryStream();

            using (output)
            {
                this.Compile(input, output, Encoding.Unicode);

                output.Seek(0, SeekOrigin.Begin);
                return Encoding.Unicode.GetString(output.ToArray());
            }
        }
        
        public void CompileFile(string inputFile, string outputFile, string encodingName = null)
        {
            if (!File.Exists(inputFile))
            {
                return;
            }
            var encoding = DetectEncoding(inputFile) ?? DefaultEncoding(encodingName);
            var input = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            var output = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.Read);

            using (output)
            {
                this.Compile(input, output, encoding);
            }
        }

        private void Compile(Stream input, Stream output, Encoding encoding)
        {
            var outputWriter = new StreamWriter(output, encoding);
            var transactionStarted = false;
            var batchStarted = false;
            using (input)
            {
                var inputReader = new StreamReader(input, encoding);
                using (inputReader)
                {
                    ICharStream inputStream = new AntlrInputStream(inputReader);
                    this.CompileStream(inputStream,
                        (o, args) =>
                        {
                            if (batchStarted)
                            {
                                // write comma only after first batch
                                outputWriter.Write("]},");
                                transactionStarted = false;
                            }
                            else
                            {
                                outputWriter.Write("[");
                            }
                            batchStarted = true;
                            outputWriter.Write(CreateHead(args.Type));
                        },
                        (o, args) =>
                        {
                            if (transactionStarted)
                            {
                                // write comma only after first transaction
                                outputWriter.Write(",");
                            }
                            transactionStarted = true;
                            outputWriter.Write(this.SerializeObject(args.Transaction));
                        });
                }
            }
            outputWriter.Write("]}]");
            outputWriter.Flush();
            output.Flush();
        }

        private static Encoding DetectEncoding(string path)
        {
            var detector = new FileCharsetDetector();
            var input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using (input)
            {
                return detector.Detect(input);
            }
        }
        
        private static Encoding DefaultEncoding(string name)
        {
            try
            {
                return name.Return(Encoding.GetEncoding, Encoding.Default);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                return Encoding.Default;
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
            this.FileStatistic = listener.FileStatistic;
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