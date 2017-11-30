using Capoala.CmdLine;
using System;

namespace Core_Console_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var specOne = new CommandLineSpecification(0, "--");
            var specTwo = new CommandLineSpecification(1, "-");
            var specThree = new CommandLineSpecification(2, "#");

            var argOne = new CommandLineArgument("one", specOne);
            var argTwo = new CommandLineArgument("two", specOne);
            var argThree = new CommandLineArgument("three", specOne);
            var argFour = new CommandLineArgument("four", specTwo);
            var argFive = new CommandLineArgument("five", specTwo);
            var argSix = new CommandLineArgument("six", specTwo);
            var argSeven = new CommandLineArgument("seven", specThree);
            var argEight = new CommandLineArgument("eight", specThree);
            var argNine = new CommandLineArgument("nine", specThree);

            var groupingOne = new CommandLineGrouping(argOne, new[] { argFour });
            var groupingTwo = new CommandLineGrouping(new[] {argOne, argFour }, new[] { argNine });

            var flags = new []
            {
                CmdLineSearchOptions.None,
                CmdLineSearchOptions.WithChildren,
                CmdLineSearchOptions.WithoutChildren,
                CmdLineSearchOptions.WithoutSiblings,
                CmdLineSearchOptions.WithSiblings,
                CmdLineSearchOptions.WithParams,
                CmdLineSearchOptions.WithoutParams,
                CmdLineSearchOptions.WithoutChildren | CmdLineSearchOptions.WithoutSiblings,     
                CmdLineSearchOptions.WithoutChildren | CmdLineSearchOptions.WithSiblings,
                CmdLineSearchOptions.WithoutChildren | CmdLineSearchOptions.WithParams,
                CmdLineSearchOptions.WithoutChildren | CmdLineSearchOptions.WithoutParams,
                CmdLineSearchOptions.WithChildren | CmdLineSearchOptions.WithoutSiblings,
                CmdLineSearchOptions.WithChildren | CmdLineSearchOptions.WithSiblings,
                CmdLineSearchOptions.WithChildren | CmdLineSearchOptions.WithParams,
                CmdLineSearchOptions.WithChildren | CmdLineSearchOptions.WithoutParams,
                CmdLineSearchOptions.WithoutChildren | CmdLineSearchOptions.WithoutSiblings | CmdLineSearchOptions.WithoutParams,
                CmdLineSearchOptions.WithoutChildren | CmdLineSearchOptions.WithoutSiblings | CmdLineSearchOptions.WithParams,
                CmdLineSearchOptions.WithoutChildren | CmdLineSearchOptions.WithSiblings | CmdLineSearchOptions.WithoutParams,
                CmdLineSearchOptions.WithoutChildren | CmdLineSearchOptions.WithSiblings | CmdLineSearchOptions.WithParams,
                CmdLineSearchOptions.WithChildren | CmdLineSearchOptions.WithSiblings | CmdLineSearchOptions.WithoutParams,
                CmdLineSearchOptions.WithChildren | CmdLineSearchOptions.WithSiblings | CmdLineSearchOptions.WithParams,
            };

            var callChain = new [] { argOne };

            foreach (var flag in flags)
                Console.WriteLine($"{callChain.Found(flag).ToString().PadRight(5)} => {flag}");

            Console.WriteLine();
            Console.WriteLine($"{argOne.Command} => {string.Join(" | ", argOne.GetParams())}");
            Console.WriteLine($"{argOne.Command} => {string.Join(" | ", new[] { argOne, argFour }.GetParams())}");
            Console.WriteLine($"{argOne.Command} => {string.Join(" | ", new[] { argOne, argFour, argNine }.GetParams())}");

            Console.WriteLine();

            foreach (var violation in new ICommandLineRestriction<CommandLineViolation>[] 
            {
                new CommandLineRestrictions.FirstArgMustBeRootRestriction(CommandLine.RootSpecification),
                new CommandLineRestrictions.UnknownArgumentsRestriction(CommandLine.KnownArguments, CommandLine.KnownSpecifications),
                new CommandLineRestrictions.MustContainAtLeastOneArgumentRestriction()
            })
                if (violation.IsViolated)
                    foreach (var violationResult in violation.GetViolations())
                        Console.WriteLine($"{violationResult.Violation} => {violationResult.Message}");
        }
    }
}
