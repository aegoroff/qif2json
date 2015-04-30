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
            using (parser)
            {
                var output = argument.Output ?? argument.Input + ".json";
                parser.CompileFile(argument.Input, output);
            }
        }
    }
}