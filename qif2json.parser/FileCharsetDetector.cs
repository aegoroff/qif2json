// Created by: egr
// Created at: 01.05.2015
// © 2015 Alexander Egorov

using System.IO;
using System.Text;
using Ude;

namespace qif2json.parser
{
    public class FileCharsetDetector
    {
        public Encoding Detect(Stream stream)
        {
            var detector = new CharsetDetector();
            detector.Feed(stream);
            detector.DataEnd();
            return detector.Charset.Return(Encoding.GetEncoding, null);
        }
    }
}