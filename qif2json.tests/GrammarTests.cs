// Created by: egr
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
^", 1, 3, false)]
        [InlineData(@"!Type:Bank
D03/03/10
T-379.00
PCITY OF SPRINGFIELD
^
!Type:Bank
D03/04/10
T-37.00
PCITY OF SPRINGFIELD
^", 2, 6, true)]
        [InlineData(@"!Type:Bank
D03/03/10
T-379.00
PCITY OF SPRINGFIELD
^
D03/04/10
T-20.28
PYOUR LOCAL SUPERMARKET
^", 2, 6, true)]
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

", 2, 10, true)]
        [InlineData(@"!Account
D03/03/10
T-379.00
^", 1, 2, true)]
        [InlineData(@"!Type:Memorized
T-50.00
POakwood Gardens
MRent
KC
^", 1, 4, true)]
        [InlineData(@"!Type:Invst
D8/25/93
NShrsIn
Yibm4
I11.260
Q88.81
CX
MOpening
^
D8/25/93
NBuyX
Yibm4
I11.030
Q9.066
T100.00
MEst. price as of 8/25/93
L[CHECKING]
$100.00
^", 2, 16, true)]
        public void Tests(string qif, long transactions, long lines, bool addId)
        {
            var parser = new QifParser(this.output.WriteLine) { Idented = true, AddId = addId};
            var json = parser.CompileString(qif);
            Assert.Equal(0, parser.NumberOfSyntaxErrors);
            this.output.WriteLine(json);
            Assert.NotEmpty(json);
            Assert.Equal(transactions, parser.FileStatistic.TotalTransactions);
            Assert.Equal(lines, parser.FileStatistic.TotalLines);
        }
    }
}
