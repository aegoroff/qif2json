namespace qif2json.parser
{
    public class Qif2JsonListener : Qif2jsonBaseListener
    {
        public string Type { get; internal set; }

        public override void ExitType(Qif2json.TypeContext context)
        {
            Type = context.TYPE().GetText();
        }

        public override void ExitAccount(Qif2json.AccountContext context)
        {
            Type = context.ACCOUNT().GetText();
        }
    }
}