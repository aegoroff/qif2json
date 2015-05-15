// Created by: egr
// Created at: 01.05.2015
// © 2015 Alexander Egorov

using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace qif2json.parser.Model
{
    public struct Transaction
    {
        private readonly List<IDictionary<string, string>> lines;

        internal Transaction(List<IDictionary<string, string>> list)
        {
            this.lines = list;
        }

        internal static Transaction Create()
        {
            return new Transaction(new List<IDictionary<string, string>>());
        }

        public IEnumerable<IDictionary<string, string>> Lines
        {
            get { return this.lines; }
        }

        internal void Add(IDictionary<string, string> line)
        {
            this.lines.Add(line);
        }

        internal void CreateIdentifier(Encoding encoding)
        {
            var data = string.Join(string.Empty, this.lines);
            var hash = Sha1HashStringForUtf8String(data, encoding);
            this.Add(new Dictionary<string, string>
            {
                {"Id", hash}
            });
        }

        private static string Sha1HashStringForUtf8String(string s, Encoding encoding)
        {
            var bytes = encoding.GetBytes(s);
            var sha1 = SHA1.Create();
            var hashBytes = sha1.ComputeHash(bytes);

            return HexStringFromBytes(hashBytes);
        }

        private static string HexStringFromBytes(IEnumerable<byte> bytes)
        {
            var sb = new StringBuilder();
            foreach (var hex in bytes.Select(b => b.ToString("x2")))
            {
                sb.Append(hex);
            }
            return sb.ToString();
        }
    }
}