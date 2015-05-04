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
            var argument = Parser.ParseIntoOrShowUsage<ProgramArguments>(args);
            if (argument == null)
            {
                return;
            }
            var parser = new QifParser
            {
                Idented = argument.Idented
            };
            var output = argument.Output ?? argument.Input + ".json";

            var sw = new Stopwatch();
            sw.Start();
            try
            {
                var fi = new FileInfo(argument.Input);
                var fs = new FileSize(fi.Length, true);
                parser.CompileFile(argument.Input, output, argument.Encoding);

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