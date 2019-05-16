namespace Capoala.CmdLine
{
    public abstract class DelimiterSpecificationBase : IDelimiterSpecification
    {
        protected DelimiterSpecificationBase(string delimiter) => Delimiter = delimiter;

        public string Delimiter { get; }
    }
}
