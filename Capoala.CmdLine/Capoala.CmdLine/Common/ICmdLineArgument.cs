namespace Capoala.CmdLine
{
    public interface ICmdLineArgument
    {
        IDelimiterSpecification Specification { get; }

        string ArgumentName { get; }
    }
}
