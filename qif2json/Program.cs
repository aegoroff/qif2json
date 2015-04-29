using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BurnSystems.CommandLine;
using qif2json.parser;

namespace qif2json
{
    class Program
    {
        static void Main(string[] args)
        {
            var argument = Parser.ParseIntoOrShowUsage<ProgramArguments>(args);
            if (argument == null)
            {
                return;
            }
            var qif = File.ReadAllText(argument.Input);
            var parser = new QifParser();
            parser.Compile(qif);
            File.WriteAllText(argument.Output, parser.ToJson(true));
        }
    }
}
