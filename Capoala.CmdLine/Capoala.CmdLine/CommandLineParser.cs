using System;
using System.Collections.Generic;
using System.Text;

namespace Capoala.CmdLine
{
    public static class CommandLineParser
    {
        public static bool Found(ICmdLineArgument argument, StringComparison comparisonType, IEnumerable<string> commandLineSegment) 
            => CoreCommandLineParser.Found(argument.Specification.Delimiter, argument.ArgumentName, comparisonType, commandLineSegment);

        public static IEnumerable<string> GetSegment(ICmdLineArgument argument, StringComparison comparisonType, IEnumerable<string> commandLineSegment)
            => CoreCommandLineParser.GetSegment(argument.Specification.Delimiter, argument.ArgumentName, comparisonType, commandLineSegment);

        public static IEnumerable<string> GetParams(ICmdLineArgument argument, StringComparison comparisonType, IEnumerable<string> commandLineSegment)
            => CoreCommandLineParser.GetParams(argument.Specification.Delimiter, argument.ArgumentName, comparisonType, commandLineSegment);
    }
}
