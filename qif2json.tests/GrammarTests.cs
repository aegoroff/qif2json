﻿// Created by: egr
// Created at: 30.04.2015
// © 2015 Alexander Egorov

using qif2json.parser;
using Xunit;
using Xunit.Abstractions;

namespace qif2json.tests
{
    public class GrammarTests
    {
        private readonly ITestOutputHelper output;

        public GrammarTests(ITestOutputHelper output)
        {
            this.output = output;
        }


        [Theory]
        [InlineData(@"!Type:Bank
D03/03/10
T-379.00
PCITY OF SPRINGFIELD
^")]
        [InlineData(@"!Type:Bank
D03/03/10
T-379.00
PCITY OF SPRINGFIELD
^
D03/04/10
T-20.28
PYOUR LOCAL SUPERMARKET
^")]
        [InlineData(@"!Type:Bank
D03/03/10
T-379.00
PCITY OF SPRINGFIELD
^
D03/04/10
T-20.28
PYOUR LOCAL SUPERMARKET
AMoscow
ALeninsky str
A
A
^

")]
        [InlineData(@"!Account
D03/03/10
T-379.00
PCITY OF SPRINGFIELD
^")]
        public void Tests(string qif)
        {
            var parser = new QifParser(output.WriteLine) { Idented = true };
            var json = parser.CompileString(qif);
            Assert.Equal(0, parser.NumberOfSyntaxErrors);
            output.WriteLine(json);
            Assert.NotEmpty(json);
        }
    }
}
