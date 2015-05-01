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
        
        public void CompileFile(string inputFile, string outputFile, string encodingName = null)
        {
            if (!File.Exists(inputFile))
            {
                return;
            }
            var encoding = DetectEncoding(inputFile) ?? DefaultEncoding(encodingName);
            var input = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            var outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.Read);
            var outputWriter = new StreamWriter(outputStream, encoding);
            var started = false;
            using (outputStream)
            {
                using (outputWriter)
                {
                    using (input)
                    {
                        var sr = new StreamReader(input, encoding);
                        using (sr)
                        {
                            ICharStream inputStream = new AntlrInputStream(sr);
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
                    }
                    outputWriter.Write("]}");
                    outputWriter.Flush();
                    outputStream.Flush();
                }
            }
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