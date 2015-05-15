// Created by: egr
// Created at: 30.04.2015
// © 2015 Alexander Egorov

using BurnSystems.CommandLine.ByAttributes;

namespace qif2json
{
    public class ProgramArguments
    {
        [UnnamedArgument(IsRequired = true, HelpText = "Path which will be compiled")]
        public string Input { get; set; }

        [UnnamedArgument(IsRequired = false, HelpText = "Path where compiled content will be stored.")]
        public string Output { get; set; }

        [NamedArgument(IsRequired = false, ShortName = 'e', HelpText = "Input file encoding if automatic detection fails.")]
        public string Encoding { get; set; }

        [NamedArgument(ShortName = 'v', HelpText = "Prints out more information")]
        public bool Verbose { get; set; }
        
        [NamedArgument(ShortName = 'i', HelpText = "Output JSON idented (not optimized)")]
        public bool Idented { get; set; }
        
        [NamedArgument(ShortName = 'd', HelpText = "Add transaction unique identifier (All transaction data SHA-1 hash)")]
        public bool AddId { get; set; }
    }
}