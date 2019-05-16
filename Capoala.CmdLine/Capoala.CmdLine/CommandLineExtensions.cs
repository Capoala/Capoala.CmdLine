using System;
using System.Collections.Generic;

namespace Capoala.CmdLine
{
    public static class CommandLineExtensions
    {
        public static bool FoundWithChild(this ICmdLineArgument argument, StringComparison comparisonType, IEnumerable<string> commandLineSegment)
            => CoreCommandLineParser.Found(argument.Specification.Delimiter, argument.ArgumentName, comparisonType, commandLineSegment);
    }
}
