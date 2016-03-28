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
                var originalInfo = new FileInfo(arguments.Input);
                var jsonInfo = new FileInfo(Path.GetFullPath(output));

                var original = ByteSize.FromBytes(originalInfo.Length);
                var json = ByteSize.FromBytes(jsonInfo.Length);
                parser.CompileFile(arguments.Input, output, arguments.Encoding);

                sw.Stop();
                Console.WriteLine(string.Empty);
              Console.WriteLine($"{original.Humanize("#.##")} original size"); // Not L10N
              Console.WriteLine($"{json.Humanize("#.##")} result size"); // Not L10N

                Console.WriteLine(string.Empty);

              Console.WriteLine($"{"account".ToQuantity((int) parser.FileStatistic.TotalAccounts)}"); // Not L10N
              Console.WriteLine($"{"transaction".ToQuantity((int) parser.FileStatistic.TotalTransactions)}"); // Not L10N
              Console.WriteLine($"{"line".ToQuantity((int) parser.FileStatistic.TotalLines)}"); // Not L10N
                Console.WriteLine(string.Empty);
              Console.WriteLine($"The result written to: {Path.GetFullPath(output)}"); // Not L10N
              Console.WriteLine($"{sw.Elapsed.Humanize()} elapsed"); // Not L10N
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}