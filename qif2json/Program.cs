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

            parser.CompileFile(argument.Input, output);

            sw.Stop();
            Console.WriteLine(string.Empty);
            Console.WriteLine("Completed. The result file is: {0}", Path.GetFullPath(output));
            Console.WriteLine("Time elapsed: {0}", sw.Elapsed);
        }
    }
}