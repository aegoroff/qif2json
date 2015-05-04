// Created by: egr
// Created at: 30.04.2015
// © 2015 Alexander Egorov

using System;
using System.Diagnostics;
using System.IO;
using BurnSystems.CommandLine;
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
                Idented = arguments.Idented
            };
            var output = arguments.Output ?? arguments.Input + ".json";

            var sw = new Stopwatch();
            sw.Start();
            try
            {
                var fi = new FileInfo(arguments.Input);
                var fs = new FileSize(fi.Length, true);
                parser.CompileFile(arguments.Input, output, arguments.Encoding);

                sw.Stop();
                Console.WriteLine(string.Empty);
                Console.WriteLine("Total accounts:         {0}", parser.FileStatistic.TotalAccounts);
                Console.WriteLine("Total transactions:     {0}", parser.FileStatistic.TotalTransactions);
                Console.WriteLine("Total lines:            {0}", parser.FileStatistic.TotalLines);
                Console.WriteLine("File size:              {0}", fs.Format());
                Console.WriteLine(string.Empty);
                Console.WriteLine("The result written to:  {0}", Path.GetFullPath(output));
                Console.WriteLine("Time elapsed:           {0}", sw.Elapsed);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}