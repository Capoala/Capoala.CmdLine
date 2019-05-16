namespace Capoala.CmdLine
{
    public abstract class CmdLineArgumentBase : ICmdLineArgument
    {
        protected CmdLineArgumentBase(IDelimiterSpecification delimiterSpecification, string argumentName)
        {
            Specification = delimiterSpecification;
            ArgumentName = argumentName;
        }

        public IDelimiterSpecification Specification { get; }

        public string ArgumentName { get; }
    }
}
