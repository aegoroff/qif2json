// Created by: egr
// Created at: 30.04.2015
// © 2015 Alexander Egorov

using System;
using System.Diagnostics;
using System.IO;
using BurnSystems.CommandLine;
using Humanizer;
using Humanizer.Bytes;
using qif2json.parser;

namespace qif2json
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Parser.ParseIntoOrShowUsage<ProgramArguments>(args).Do(Complie);
        }

        private static void Complie(ProgramArguments arguments)
        {
            var parser = new QifParser
            {
                Idented = arguments.Idented, 
                AddId = arguments.AddId
            };
            var output = arguments.Output ?? arguments.Input + ".json"; // Not L10N

            var sw = new Stopwatch();
            sw.Start();
            try
            {
                var fi = new FileInfo(arguments.Input);
                var bs = ByteSize.FromBytes(fi.Length);
                parser.CompileFile(arguments.Input, output, arguments.Encoding);

                sw.Stop();
                Console.WriteLine(string.Empty);
                Console.WriteLine("{0}", bs.Humanize("#.##")); // Not L10N
                Console.WriteLine("{0}", "account".ToQuantity((int)parser.FileStatistic.TotalAccounts)); // Not L10N
                Console.WriteLine("{0}", "transaction".ToQuantity((int)parser.FileStatistic.TotalTransactions)); // Not L10N
                Console.WriteLine("{0}", "line".ToQuantity((int)parser.FileStatistic.TotalLines)); // Not L10N
                Console.WriteLine(string.Empty);
                Console.WriteLine("The result written to: {0}", Path.GetFullPath(output)); // Not L10N
                Console.WriteLine("{0} elapsed", sw.Elapsed.Humanize()); // Not L10N
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}