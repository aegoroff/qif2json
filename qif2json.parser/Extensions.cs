// Created by: egr
// Created at: 01.05.2015
// © 2015 Alexander Egorov

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace qif2json.parser
{
    public static class Extensions
    {
        /// <summary>
        /// With monad that defines failure result
        /// </summary>
        /// <typeparam name="TInput">Input type</typeparam>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="input">Input instance</param>
        /// <param name="evaluator">Evaluation function</param>
        /// <param name="failure">Failure result</param>
        /// <returns>if input null returns null evaluator result otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult Return<TInput, TResult>(this TInput input, Func<TInput, TResult> evaluator, TResult failure)
            where TInput : class
        {
            return input == null ? failure : evaluator(input);
        }

        public static string FormatString(this ulong value)
        {
            if (value == 0)
            {
                return string.Empty;
            }
            Func<ulong, int> count = null;
            count = num => (num /= 10) > 0 ? 1 + count(num) : 1;

            var digits = count(value);

            var builder = new StringBuilder();
            for (var i = digits; i > 0; i--)
            {
                if (i % 3 == 0 && i < digits)
                {
                    builder.Append(' ');
                }
                builder.Append('#');
            }
            return builder.ToString();
        }
        private static string BytesToString(this ulong bytes)
        {
            return ((long)bytes).DeclensionEn("byte", "bytes");
        }

        private static string DeclensionEn(this long number, string nominative, string genitivePlural)
        {
            if (number == 1 || number == -1)
            {
                return nominative;
            }
            return genitivePlural;
        }

        private const string BigFileFormat = "{0:F2} {1} ({2} {3})";
        private const string BigFileFormatNoBytes = "{0:F2} {1}";
        private const string SmallFileFormat = "{0} {1}";

        public static string Format(this FileSize fileSize)
        {
            string[] sizes =
            {
                "bytes",
                "Kb",
                "Mb",
                "Gb",
                "Tb",
                "Pb",
                "Eb"
            };
            if (fileSize.Unit == SizeUnit.Bytes)
            {
                return string.Format(CultureInfo.CurrentCulture, SmallFileFormat, fileSize.Bytes,
                    fileSize.Bytes.BytesToString());
            }
            if (fileSize.BigWithoutBytes)
            {
                return string.Format(CultureInfo.CurrentCulture, BigFileFormatNoBytes, fileSize.Value, sizes[(int)fileSize.Unit]);
            }
            return string.Format(CultureInfo.CurrentCulture, BigFileFormat, fileSize.Value,
                sizes[(int)fileSize.Unit], fileSize.Bytes.ToString(fileSize.Bytes.FormatString(), CultureInfo.CurrentCulture),
                fileSize.Bytes.BytesToString());
        }
    }
}