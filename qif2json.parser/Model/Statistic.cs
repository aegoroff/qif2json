// Created by: egr
// Created at: 01.05.2015
// © 2015 Alexander Egorov

namespace qif2json.parser.Model
{
    public class Statistic
    {
        public long TotalTransactions { get; set; }
        
        public long TotalLines { get; set; }
        
        public string FileType { get; set; }
    }
}