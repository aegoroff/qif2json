// Created by: egr
// Created at: 30.04.2015
// © 2015 Alexander Egorov

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Newtonsoft.Json;
using qif2json.parser.Model;

#if DEBUG
using qif2json.parser.Debug;
#endif

namespace qif2json.parser
{
    public class QifParser
    {
#if DEBUG
        private readonly Action<string> output;
#endif
        readonly Dictionary<AccountType, Dictionary<string, string>> allowedCodes = new Dictionary<AccountType, Dictionary<string, string>>();

        public QifParser(Action<string> output = null)
        {
#if DEBUG
            this.output = output;
#endif
            var nonInvestmentCodes = new[] { "D", "T", "C", "N", "P", "M", "A", "L", "S", "E", "$" }.ToDictionary(s => s);
            var investmentCodes = new[] { "D", "N", "Y", "I", "Q", "T", "C", "P", "M", "O", "L", "$" }.ToDictionary(s => s);
            var accountCodes = new[] { "N", "T", "D", "L", "/", "$" }.ToDictionary(s => s);
            var catListCodes = new[] { "N", "D", "T", "I", "E", "B", "R" }.ToDictionary(s => s);
            var classCodes = new[] { "N", "D" }.ToDictionary(s => s);
            var memTranList = new[] { "KC", "KD", "KP", "KI", "KE", "T", "C", "P", "M", "A", "L", "S", "E", "$", "1", "2", "3", "4", "5", "6", "7" }.ToDictionary(s => s);
            this.allowedCodes.Add(AccountType.NonInvestment, nonInvestmentCodes);
            this.allowedCodes.Add(AccountType.Investment, investmentCodes);
            this.allowedCodes.Add(AccountType.AccountInformation, accountCodes);
            this.allowedCodes.Add(AccountType.CategoryList, catListCodes);
            this.allowedCodes.Add(AccountType.ClassList, classCodes);
            this.allowedCodes.Add(AccountType.MemorizedTransactionList, memTranList);
        }


        public int NumberOfSyntaxErrors { get; private set; }
        
        public bool Idented { get; set; }

        public Statistic FileStatistic { get; private set; }


        public string CompileString(string qifString)
        {
            var inStream = new MemoryStream(Encoding.Unicode.GetBytes(qifString));
            var outStream = new MemoryStream();

            using (outStream)
            {
                this.Compile(inStream, outStream, Encoding.Unicode);

                outStream.Seek(0, SeekOrigin.Begin);
                return Encoding.Unicode.GetString(outStream.ToArray());
            }
        }
        
        public void CompileFile(string inputFile, string outputFile, string encodingName = null)
        {
            if (!File.Exists(inputFile))
            {
                return;
            }
            var encoding = DetectEncoding(inputFile) ?? DefaultEncoding(encodingName);
            var inStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            var outStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.Read);

            using (outStream)
            {
                this.Compile(inStream, outStream, encoding);
            }
        }

        private void Compile(Stream inStream, Stream outStream, Encoding encoding)
        {
            var outputWriter = new StreamWriter(outStream, encoding);
            var transactionStarted = false;
            var batchStarted = false;
            using (inStream)
            {
                var inputReader = new StreamReader(inStream, encoding);
                using (inputReader)
                {
                    ICharStream inputStream = new AntlrInputStream(inputReader);
                    this.CompileStream(inputStream,
                        (o, args) =>
                        {
                            if (batchStarted)
                            {
                                // write comma only after first batch
                                outputWriter.Write("]},");
                                transactionStarted = false;
                            }
                            else
                            {
                                outputWriter.Write("[");
                            }
                            batchStarted = true;
                            outputWriter.Write(CreateHead(args.Element));
                        },
                        (o, args) =>
                        {
                            if (transactionStarted)
                            {
                                // write comma only after first transaction
                                outputWriter.Write(",");
                            }
                            transactionStarted = true;
                            outputWriter.Write(this.SerializeObject(args.Transaction));
                        });
                }
            }
            outputWriter.Write("]}]");
            outputWriter.Flush();
            outStream.Flush();
        }

        private static Encoding DetectEncoding(string path)
        {
            var detector = new FileCharsetDetector();
            var input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using (input)
            {
                return detector.Detect(input);
            }
        }
        
        private static Encoding DefaultEncoding(string name)
        {
            try
            {
                return name.Return(Encoding.GetEncoding, Encoding.Default);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                return Encoding.Default;
            }
        }

        private void CompileStream(
            ICharStream inputStream, 
            Action<object, SyntaxElementEventArgs> onTypeDetect, 
            Action<object, TransactionDetectedEventArgs> onTransactionDetect)
        {
            var lexer = new Qif2jsonLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Qif2json(tokenStream);

#if DEBUG
            parser.Trace = true;
            if (output != null)
            {
                var treeL = new TreeListener(output, parser);
                parser.AddParseListener(treeL);
                parser.AddErrorListener(new ErrorListener(output, treeL));
            }
#endif
            NumberOfSyntaxErrors = parser.NumberOfSyntaxErrors;
            if (NumberOfSyntaxErrors > 0)
            {
                return;
            }

            var listener = new Qif2JsonListener();
            listener.TransactionDetected += (sender, e) => onTransactionDetect(sender, e);
            listener.TypeDetected += (sender, e) => onTypeDetect(sender, e);
            listener.LineCodeDetected += this.OnLineCodeDetected;
            parser.AddParseListener(listener);
            parser.compileUnit();
            this.FileStatistic = listener.FileStatistic;
        }

        private void OnLineCodeDetected(object sender, SyntaxElementEventArgs args)
        {
            if (!this.allowedCodes[currentType].ContainsKey(args.Element))
            {
                throw new NotSupportedException(string.Format("Code {0} not supported for {1} account type", args.Element, currentType));
            }
        }


        /*
         * Items for Non-Investment Accounts 
Each item in a bank, cash, credit card, other liability, or other asset account must begin with a letter that indicates the field in the Quicken register. 
The non-split items can be in any sequence:  
D	Date
T	Amount
C	Cleared status
N	Num (check or reference number)
P	Payee
M	Memo
A	Address (up to five lines; the sixth line is an optional message)
L	Category (Category/Subcategory/Transfer/Class)
S	Category in split (Category/Transfer/Class)
E	Memo in split
$	Dollar amount of split


Items for Investment Accounts 

D	Date
N	Action
Y	Security
I	Price
Q	Quantity (number of shares or split ratio)
T	Transaction amount
C	Cleared status
P	Text in the first line for transfers and reminders
M	Memo
O	Commission
L	Account for the transfer
$	Amount transferred

Items for Account Information 

The account header !Account is used in two places-at the start of an account list and the start of a list of transactions to specify to which account they belong. 
Field	Indicator Explanation
N	Name
T	Type of account
D	Description
L	Credit limit (only for credit card accounts)
/	Statement balance date
$	Statement balance amount

Items for a Category List

Field	Indicator Explanation
N	Category name:subcategory name
D	Description
T	Tax related if included, not tax related if omitted
I	Income category
E	Expense category (if category type is unspecified, quicken assumes expense type)
B	Budget amount (only in a Budget Amounts QIF file)
R	Tax schedule information

Items for a Class List

Field	Indicator Explanation
N	Class name
D	Description

Items for a Memorized Transaction List

Immediately preceding the ^ character, each entry must end with one of the following file indicators to specify the transaction type. 
KC
KD
KP
KI
KE
With that exception, memorized transaction entries have the same format as regular transaction entries (non-investment accounts). 
However, the Date or Num field is included. All items are optional, but if an amortization record is included, all seven amortization lines must also be included.
Field	Indicator Explanation
KC	Check transaction
KD	Deposit transaction
KP	Payment transaction
KI	Investment transaction
KE	Electronic payee transaction
T	Amount
C	Cleared status
P	Payee
M	Memo
A	Address
L	Category or Transfer/Class
S	Category/class in split
E	Memo in split
$	Dollar amount of split
1	Amortization: First payment date
2	Amortization: Total years for loan
3	Amortization: Number of payments already made
4	Amortization: Number of periods per year
5	Amortization: Interest rate
6	Amortization: Current loan balance
7	Amortization: Original loan amount

         */

        private string SerializeObject(object json)
        {
            return JsonConvert.SerializeObject(json, this.Idented ? Formatting.Indented : Formatting.None);
        }

        private AccountType currentType;

        private static AccountType ToAccountType(string type)
        {
            switch (type)
            {
                case "Invst":
                    return AccountType.Investment;
                case "Account":
                    return AccountType.AccountInformation;
                case "Cat":
                    return AccountType.CategoryList;
                case "Class":
                    return AccountType.ClassList;
                case "Memorized":
                    return AccountType.MemorizedTransactionList;
                default:
                    return AccountType.NonInvestment;
            }
        }

        private string CreateHead(string type)
        {
            this.currentType = ToAccountType(type);
            return "{" + string.Format("\"Type\": \"{0}\", \"Transactions\": [", type);
        }
    }
}