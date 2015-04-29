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
^", "Bank")]
        [InlineData(@"!Type:Bank
D03/03/10
T-379.00
PCITY OF SPRINGFIELD
^
D03/04/10
T-20.28
PYOUR LOCAL SUPERMARKET
^", "Bank")]
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
^", "Bank")]
        [InlineData(@"!Account
D03/03/10
T-379.00
PCITY OF SPRINGFIELD
^", "Account")]
        public void Tests(string qif, string type)
        {
            var parser = new QifParser(output.WriteLine);
            parser.Compile(qif);
            Assert.Equal(0, parser.NumberOfSyntaxErrors);
            Assert.Equal(type, parser.FileType);
        }
    }
}
