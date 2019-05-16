using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Capoala.CmdLine
{
    public static class CommandLineHelper
    {
        public static IEnumerable<string> Arguments => Environment.GetCommandLineArgs();

        public static string ArgumentsString => string.Join(" ", Arguments);


        public static bool Found(string delimiter, string argument, StringComparison stringComparison, IEnumerable<string> commandLine) => CoreCommandLineParser.Found(delimiter, argument, stringComparison, commandLine);

        public static bool Found(string delimiter, string argument, StringComparison stringComparison, string commandLine) => Found(delimiter, argument, stringComparison, commandLine.Split(' '));
    }
}
