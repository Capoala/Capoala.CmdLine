using System.Collections.Generic;
using System.Linq;

namespace Capoala.CmdLine
{
    /// <summary>
    /// Contains default, common, <see cref="ICommandLineRestriction{T}"/> implementations.
    /// </summary>
    public static class CommandLineRestrictions
    {
        /// <summary>
        /// A restriction which specifies legal arguments.
        /// </summary>
        public sealed class LegalArgumentsRestriction : CommandLineRestrictionBase<CommandLineViolation>
        {
            /// <summary>
            /// A collection of <see cref="ICommandLineGrouping"/> representing legal call-chains and associated child <see cref="ICommandLineArgument"/>.
            /// </summary>
            public ICommandLineGrouping[] LegalCombinations { get; }

            /// <summary>
            /// A collection of all known <see cref="ICommandLineArgument"/>.
            /// </summary>
            public IEnumerable<ICommandLineArgument> KnownArguments { get; }

            /// <summary>
            /// A collection of all known <see cref="ICommandLineSpecification"/>.
            /// </summary>
            public IEnumerable<ICommandLineSpecification> KnownSpecifications { get; }

            /// <summary>
            /// The root specification.
            /// </summary>
            public ICommandLineSpecification RootSpecification { get; }

            /// <summary>
            /// Creates a new instance of <see cref="LegalArgumentsRestriction"/>.
            /// </summary>
            /// <param name="knownArguments">A collection of all known <see cref="ICommandLineArgument"/>.</param>
            /// <param name="knownSpecifications">A collection of all known <see cref="ICommandLineSpecification"/>.</param>
            /// <param name="legalCombinations">A collection of <see cref="ICommandLineGrouping"/> representing legal call-chains and associated child <see cref="ICommandLineArgument"/>.</param>
            public LegalArgumentsRestriction(IEnumerable<ICommandLineArgument> knownArguments, IEnumerable<ICommandLineSpecification> knownSpecifications, params ICommandLineGrouping[] legalCombinations)
            {
                KnownArguments = knownArguments;
                KnownSpecifications = knownSpecifications;
                RootSpecification = knownSpecifications.Where(ks => ks.Hierarchy == knownSpecifications.Select(kspec => kspec.Hierarchy).Min()).FirstOrDefault();
                LegalCombinations = legalCombinations;
            }

            /// <summary>
            /// Performs all necessary queries to determine whether any violations were found, returning all violation objects as <see cref="CommandLineViolation"/>.
            /// </summary>
            /// <returns>
            /// Returns a collection of <see cref="CommandLineViolation"/> representing the violations, if any violations where found.
            /// Returns an empty collection if no violations were found.
            /// </returns>
            public override IEnumerable<CommandLineViolation> GetViolations()
            {
                IEnumerable<string> rootArgs = CommandLine.Arguments.Where(str => str.StartsWith(RootSpecification.Delimiter));
                IEnumerable<ICommandLineArgument> rootICLAs = KnownArguments.Where(icla => rootArgs.Contains(icla.Command));
                IEnumerable<string> workingCollection = CommandLine.ExcludeParams(CommandLine.Arguments, KnownSpecifications);
                List<IEnumerable<string>> rootSegments = new List<IEnumerable<string>>();

                foreach (ICommandLineArgument rootArg in rootICLAs)
                {
                    var segment = rootArg.GetSegment(null, workingCollection);

                    if (segment.Any())
                    {
                        rootSegments.Add(segment);
                        workingCollection = workingCollection.ExcludeSubset(segment);
                    }
                }

                foreach (ICommandLineGrouping grouping in LegalCombinations)
                {
                    IEnumerable<string> groupCommands = grouping.ParentCallChain.Select(pa => pa.Command)
                                                                .Concat(grouping.Children.Select(child => child.Command));

                    foreach (IEnumerable<string> rootSegment in rootSegments)
                    {
                        IEnumerable<string> rootSeg = rootSegment;

                        if (groupCommands.UnorderedSequenceEquals(rootSeg))
                        {
                            rootSegments.Remove(rootSegment);
                            break;
                        }
                    }
                }

                if (rootSegments.Any())
                    foreach (IEnumerable<string> rootSegment in rootSegments)
                        yield return new CommandLineViolation() { Violation = nameof(LegalArgumentsRestriction), Message = string.Join(" ", rootSegment) };
            }
        }

        /// <summary>
        /// A restriction which restricts unknown arguments.
        /// </summary>
        public sealed class UnknownArgumentsRestriction : CommandLineRestrictionBase<CommandLineViolation>
        {
            /// <summary>
            /// A collection of all known <see cref="ICommandLineArgument"/>.
            /// </summary>
            public IEnumerable<ICommandLineArgument> KnownArguments { get; }

            /// <summary>
            /// A collection of all known <see cref="ICommandLineSpecification"/>.
            /// </summary>
            public IEnumerable<ICommandLineSpecification> KnownSpecifications { get; }

            /// <summary>
            /// Creates a new <see cref="UnknownArgumentsRestriction"/> instance.
            /// </summary>
            /// <param name="knownArguments">A collection of all known <see cref="ICommandLineArgument"/>.</param>
            /// <param name="knownSpecifications">A collection of all known <see cref="ICommandLineSpecification"/>.</param>
            public UnknownArgumentsRestriction(IEnumerable<ICommandLineArgument> knownArguments, IEnumerable<ICommandLineSpecification> knownSpecifications)
            {
                KnownArguments = knownArguments;
                KnownSpecifications = knownSpecifications;
            }

            /// <summary>
            /// Performs all necessary queries to determine whether any violations were found, returning all violation objects as <see cref="CommandLineViolation"/>.
            /// </summary>
            /// <returns>
            /// Returns a collection of <see cref="CommandLineViolation"/> representing the violations, if any violations where found.
            /// Returns an empty collection if no violations were found.
            /// </returns>
            public override IEnumerable<CommandLineViolation> GetViolations()
            {
                if (CommandLine.Arguments.Any())
                {
                    var unknownArgs = CommandLine.ExcludeParams(CommandLine.Arguments, KnownSpecifications).Except(KnownArguments.Select(icla => icla.Command));
                    if (unknownArgs.Any())
                        foreach (string unknownArg in unknownArgs)
                            yield return new CommandLineViolation() { Violation = nameof(UnknownArgumentsRestriction), Message = unknownArg };
                }
            }
        }

        /// <summary>
        /// A restriction which enforces the hierarchal order.
        /// </summary>
        public sealed class FirstArgMustBeRootRestriction : CommandLineRestrictionBase<CommandLineViolation>
        {
            /// <summary>
            /// The root specification.
            /// </summary>
            public ICommandLineSpecification RootSpecification { get; }

            /// <summary>
            /// Creates a new <see cref="FirstArgMustBeRootRestriction"/> instance.
            /// </summary>
            /// <param name="rootSpecification">The root specification.</param>
            public FirstArgMustBeRootRestriction(ICommandLineSpecification rootSpecification) => RootSpecification = rootSpecification;

            /// <summary>
            /// Performs all necessary queries to determine whether any violations were found, returning all violation objects as <see cref="CommandLineViolation"/>.
            /// </summary>
            /// <returns>
            /// Returns a collection of <see cref="CommandLineViolation"/> representing the violations, if any violations where found.
            /// Returns an empty collection if no violations were found.
            /// </returns>
            public override IEnumerable<CommandLineViolation> GetViolations()
            {
                if (CommandLine.Arguments.Any())
                    if (!CommandLine.Arguments.FirstOrDefault().StartsWith(RootSpecification.Delimiter))
                        yield return new CommandLineViolation() { Violation = nameof(FirstArgMustBeRootRestriction), Message = CommandLine.Arguments.FirstOrDefault() };
            }
        }

        /// <summary>
        /// A restriction which specifies how many parameters an <see cref="ICommandLineArgument"/> should have.
        /// </summary>
        public sealed class ParameterCountRestriction : CommandLineRestrictionBase<CommandLineViolation>
        {
            /// <summary>
            /// The call-chain.
            /// </summary>
            public ICommandLineArgument[] CallChain { get; private set; }

            /// <summary>
            /// The minimum number of required parameters.
            /// </summary>
            public int MinParamsAllowed { get; private set; }

            /// <summary>
            /// The maximum number of allowed parameters.
            /// </summary>
            public int MaxParamsAllowed { get; private set; }

            /// <summary>
            /// Creates a new instance of <see cref="ParameterCountRestriction"/>.
            /// </summary>
            /// <param name="min">The minimum number of required parameters.</param>
            /// <param name="max">The maximum number of allowed parameters.</param>
            /// <param name="callChain">The call-chain.</param>
            public ParameterCountRestriction(int min, int max, params ICommandLineArgument[] callChain)
            {
                CallChain = callChain;
                MinParamsAllowed = min;
                MaxParamsAllowed = max;
            }

            /// <summary>
            /// Performs all necessary queries to determine whether any violations were found, returning all violation objects as <see cref="CommandLineViolation"/>.
            /// </summary>
            /// <returns>
            /// Returns a collection of <see cref="CommandLineViolation"/> representing the violations, if any violations where found.
            /// Returns an empty collection if no violations were found.
            /// </returns>
            public override IEnumerable<CommandLineViolation> GetViolations()
            {
                if (CommandLine.Arguments.Any() && CallChain.Found(CmdLineSearchOptions.None))
                {

                    int paramCount = CallChain.GetParams().Count();
                    if (paramCount < MinParamsAllowed)
                        yield return new CommandLineViolation() { Violation = nameof(ParameterCountRestriction), Message = string.Join(" ", CallChain.Select(icla => icla.Command)) };
                    else if (paramCount > MaxParamsAllowed)
                        yield return new CommandLineViolation() { Violation = nameof(ParameterCountRestriction), Message = string.Join(" ", CallChain.Select(icla => icla.Command)) };
                }
            }
        }

        /// <summary>
        /// A restriction which specifies illegal combinations.
        /// </summary>
        public sealed class IllegalComboRestriction : CommandLineRestrictionBase<CommandLineViolation>
        {
            /// <summary>
            /// The first grouping.
            /// </summary>
            public ICommandLineGrouping FirstGrouping { get; private set; }

            /// <summary>
            /// The second grouping.
            /// </summary>
            public ICommandLineGrouping SecondGrouping { get; private set; }

            /// <summary>
            /// Creates a new instance of <see cref="IllegalComboRestriction"/>.
            /// </summary>
            /// <param name="firstGrouping">The first grouping.</param>
            /// <param name="secondGrouping">The second grouping.</param>
            public IllegalComboRestriction(ICommandLineGrouping firstGrouping, ICommandLineGrouping secondGrouping)
            {
                FirstGrouping = firstGrouping;
                SecondGrouping = secondGrouping;
            }

            /// <summary>
            /// Performs all necessary queries to determine whether any violations were found, returning all violation objects as <see cref="CommandLineViolation"/>.
            /// </summary>
            /// <returns>
            /// Returns a collection of <see cref="CommandLineViolation"/> representing the violations, if any violations where found.
            /// Returns an empty collection if no violations were found.
            /// </returns>
            public override IEnumerable<CommandLineViolation> GetViolations()
            {
                if (CommandLine.Arguments.Any())
                {
                    IEnumerable<ICommandLineArgument> firstChain = FirstGrouping.ParentCallChain.Concat(FirstGrouping.Children);
                    IEnumerable<ICommandLineArgument> secondChain = SecondGrouping.ParentCallChain.Concat(SecondGrouping.Children);
                    if (FirstGrouping.Found(CmdLineSearchOptions.None) && SecondGrouping.Found(CmdLineSearchOptions.None))
                    {
                        yield return new CommandLineViolation()
                        {
                            Violation = nameof(IllegalComboRestriction),
                            Message = $"{string.Join(" ", firstChain.Select(icla => icla.Command))} and {string.Join(" ", secondChain.Select(icla => icla.Command))}"
                        };
                    }
                }
            }
        }

        /// <summary>
        /// A restriction which specifies mandated combinations.
        /// </summary>
        public sealed class MandatedComboRestriction : CommandLineRestrictionBase<CommandLineViolation>
        {
            /// <summary>
            /// The first grouping.
            /// </summary>
            public ICommandLineGrouping FirstGrouping { get; private set; }

            /// <summary>
            /// The second grouping.
            /// </summary>
            public ICommandLineGrouping SecondGrouping { get; private set; }

            /// <summary>
            /// Creates a new instance of <see cref="IllegalComboRestriction"/>.
            /// </summary>
            /// <param name="firstGrouping">The first grouping.</param>
            /// <param name="secondGrouping">The second grouping.</param>
            public MandatedComboRestriction(ICommandLineGrouping firstGrouping, ICommandLineGrouping secondGrouping)
            {
                FirstGrouping = firstGrouping;
                SecondGrouping = secondGrouping;
            }

            /// <summary>
            /// Performs all necessary queries to determine whether any violations were found, returning all violation objects as <see cref="CommandLineViolation"/>.
            /// </summary>
            /// <returns>
            /// Returns a collection of <see cref="CommandLineViolation"/> representing the violations, if any violations where found.
            /// Returns an empty collection if no violations were found.
            /// </returns>
            public override IEnumerable<CommandLineViolation> GetViolations()
            {
                if (CommandLine.Arguments.Any())
                {
                    IEnumerable<ICommandLineArgument> firstChain = FirstGrouping.ParentCallChain.Concat(FirstGrouping.Children);
                    IEnumerable<ICommandLineArgument> secondChain = SecondGrouping.ParentCallChain.Concat(SecondGrouping.Children);
                    bool firstPresent = FirstGrouping.Found(CmdLineSearchOptions.None);
                    bool secondPresent = SecondGrouping.Found(CmdLineSearchOptions.None);
                    if ((firstPresent || secondPresent) && !(firstPresent && secondPresent))
                    {
                        yield return new CommandLineViolation()
                        {
                            Violation = nameof(MandatedComboRestriction),
                            Message = $"{string.Join(" ", firstChain.Select(icla => icla.Command))} and {string.Join(" ", secondChain.Select(icla => icla.Command))}"
                        };
                    }
                }
            }
        }

        /// <summary>
        /// A restriction which specifies that at least one argument must be supplied.
        /// </summary>
        public sealed class MustContainAtLeastOneArgumentRestriction : CommandLineRestrictionBase<CommandLineViolation>
        {
            /// <summary>
            /// Performs all necessary queries to determine whether any violations were found, returning all violation objects as <see cref="CommandLineViolation"/>.
            /// </summary>
            /// <returns>
            /// Returns a collection of <see cref="CommandLineViolation"/> representing the violations, if any violations where found.
            /// Returns an empty collection if no violations were found.
            /// </returns>
            public override IEnumerable<CommandLineViolation> GetViolations()
            {
                if (!CommandLine.Arguments.Any())
                {
                    yield return new CommandLineViolation()
                    {
                        Violation = nameof(MustContainAtLeastOneArgumentRestriction),
                        Message = "No argument were found."
                    };
                }
            }
        }

        /// <summary>
        /// A restriction which specifies that at least one argument must be supplied.
        /// </summary>
        public sealed class CannotContainAnyArgumentsRestriction : CommandLineRestrictionBase<CommandLineViolation>
        {
            /// <summary>
            /// Performs all necessary queries to determine whether any violations were found, returning all violation objects as <see cref="CommandLineViolation"/>.
            /// </summary>
            /// <returns>
            /// Returns a collection of <see cref="CommandLineViolation"/> representing the violations, if any violations where found.
            /// Returns an empty collection if no violations were found.
            /// </returns>
            public override IEnumerable<CommandLineViolation> GetViolations()
            {
                if (!CommandLine.Arguments.Any())
                {
                    yield return new CommandLineViolation()
                    {
                        Violation = nameof(CannotContainAnyArgumentsRestriction),
                        Message = "One or more arguments were found."
                    };
                }
            }
        }
    }
}
