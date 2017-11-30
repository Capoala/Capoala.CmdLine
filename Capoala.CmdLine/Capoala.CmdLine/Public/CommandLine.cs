using System;
using System.Collections.Generic;
using System.Linq;

namespace Capoala.CmdLine
{
    /// <summary>
    /// Provides the means to create, retrieve, and configure command line arguments. This class cannot be inherited.
    /// </summary>
    public static class CommandLine
    {
        /// <summary>
        /// Specifies how arguments are compared to one another.
        /// </summary>
        public static StringComparison Comparer { get; set; } = StringComparison.OrdinalIgnoreCase;

        /// <summary>
        /// Returns the received command line arguments, minus the path to the executing application.
        /// </summary>
        public static IEnumerable<string> Arguments => Environment.GetCommandLineArgs().Skip(1);

        /// <summary>
        /// Returns the received command line arguments as a <see cref="string"/>, minus the path to the executing application.
        /// </summary>
        public static string ArgumentsString => string.Join(" ", Arguments);

        /// <summary>
        /// A collection of instatiated <see cref="ICommandLineSpecification"/> objects.
        /// </summary>
        public static IEnumerable<ICommandLineSpecification> KnownSpecifications => CommandLineSetManager.KnownSpecifications.AsEnumerable();

        /// <summary>
        /// A collection of instatiated <see cref="ICommandLineArgument"/> objects.
        /// </summary>
        public static IEnumerable<ICommandLineArgument> KnownArguments => CommandLineSetManager.KnownArguments.AsEnumerable();

        /// <summary>
        /// The root <see cref="ICommandLineSpecification"/>.
        /// <para>
        /// The root is defined as the object with the lowest <see cref="ICommandLineSpecification.Hierarchy"/> value.
        /// </para>
        /// </summary>
        public static ICommandLineSpecification RootSpecification => CommandLineSetManager.KnownSpecifications.FirstOrDefault(spec => spec.Hierarchy == CommandLineSetManager.KnownSpecifications.Select(knownSpec => knownSpec.Hierarchy).Min());

        #region GetSegment
        /// <summary>
        /// Returns a collection of values starting at <paramref name="argument"/>, and ending at the next value that begins 
        /// with the first character of <paramref name="argument"/>.
        /// </summary>
        /// <param name="argument">The argument to search for.</param>
        /// <param name="comparer">The comparer to use when determining if a match was found.</param>
        /// <param name="argumentsToParse">
        /// The collection to search. 
        /// Specifying null will default to <see cref="CommandLine.Arguments"/>.
        /// </param>
        /// <returns>
        /// Returns all child <see cref="ICommandLineArgument.Command"/> and parameters based upon the 
        /// <see cref="ICommandLineSpecification"/> if <paramref name="argument"/> is found.
        /// Returns an empty collection if <paramref name="argument"/> is not found.
        /// </returns>
        /// <remarks>
        /// This method searches <paramref name="argumentsToParse"/> for a value that matches <paramref name="argument"/> 
        /// using the <see cref="Comparer"/> value.
        /// Once found, that value and all proceeding values are returned until a value whose first character matches that 
        /// of the <paramref name="argument"/>'s delimiter.
        /// 
        /// If <paramref name="argumentsToParse"/> is null, <see cref="Arguments"/> is used.
        /// 
        /// If no matches to <paramref name="argument"/> are found, an empty collection is returned.
        /// </remarks>
        public static IEnumerable<string> GetSegment(this ICommandLineArgument argument, StringComparison? comparer = null, IEnumerable<string> argumentsToParse = null)
        {
            var workingSegment = argumentsToParse ?? Arguments;
            var workingComparer = comparer ?? Comparer;
            bool didFindFirst = false;
            if (workingSegment.Any(str => argument.Command.Equals(str, workingComparer)))
            {
                foreach (var arg in workingSegment)
                {
                    if (!didFindFirst)
                    {
                        if (argument.Command.Equals(arg, workingComparer))
                        {
                            yield return arg;
                            didFindFirst = true;
                        }
                    }
                    else
                    {
                        if (arg.StartsWith(argument.Specification.Delimiter))
                            break;
                        else
                            yield return arg;
                    }
                }
            }
        }

        /// <summary>
        /// Returns a collection of child arguments and parameters of the given call-chain.
        /// </summary>
        /// <param name="callChain">The call-chain.</param>
        /// <param name="comparer">The comparer to use when determining if a match was found.</param>
        /// <param name="argumentsToParse">
        /// The collection to search. 
        /// Specifying null will default to <see cref="Arguments"/>.
        /// </param>
        /// <returns>
        /// Returns all child <see cref="ICommandLineArgument.Command"/> and parameters based upon the 
        /// <see cref="ICommandLineSpecification"/> if <paramref name="callChain"/> is found.
        /// Returns an empty collection if <paramref name="callChain"/> is not found.
        /// </returns>
        /// <remarks>
        /// This method searches <paramref name="argumentsToParse"/> for a value that matches <paramref name="callChain"/> 
        /// using the <see cref="Comparer"/> value.
        /// Once found, that value and all proceeding values are returned until a value whose first character matches that 
        /// of the <paramref name="callChain"/>'s last argument's delimiter.
        /// 
        /// If <paramref name="argumentsToParse"/> is null, <see cref="Arguments"/> is used.
        /// 
        /// If no matches to <paramref name="callChain"/> are found, an empty collection is returned.
        /// </remarks>
        public static IEnumerable<string> GetSegment(this IEnumerable<ICommandLineArgument> callChain, StringComparison? comparer = null, IEnumerable<string> argumentsToParse = null)
        {
            var workingSegment = argumentsToParse ?? Arguments;
            var workingComparer = comparer ?? Comparer;
            if (workingSegment.Any())
                foreach (var cmdLineArg in callChain)
                    workingSegment = cmdLineArg.GetSegment(workingComparer, workingSegment);
            return workingSegment;
        }
        #endregion

        #region GetParams
        /// <summary>
        /// Returns all parameters associated with the specified <see cref="ICommandLineArgument"/>.
        /// </summary>
        /// <param name="argument">The argument to retrieve the parameters of.</param>
        /// <param name="comparer">The comparer to use when determining if a match was found.</param>
        /// <param name="argumentsToParse">
        /// The collection to search. 
        /// Specifying null will default to <see cref="Arguments"/>.
        /// </param>
        /// <returns>
        /// Searches <paramref name="argumentsToParse"/> for <paramref name="argument"/>. 
        /// Once found, proceeding values are returned until a value begins with a known <see cref="ICommandLineSpecification.Delimiter"/>.
        /// 
        /// If <paramref name="argument"/> is not found, or no parameters are found, an empty collection is returned.
        /// </returns>
        public static IEnumerable<string> GetParams(this ICommandLineArgument argument, StringComparison? comparer = null, IEnumerable<string> argumentsToParse = null)
            => argument.GetSegment(comparer, argumentsToParse ?? Arguments)
                       .Skip(1)
                       .TakeWhile(arg => !CommandLineSetManager.KnownSpecifications.Any(ks => arg.StartsWith(ks.Delimiter)));

        /// <summary>
        /// Returns all parameters associated with the specified call-chain.
        /// </summary>
        /// <param name="callChain">The call-chain to retrieve the parameters of.</param>
        /// <param name="comparer">The comparer to use when determining if a match was found.</param>
        /// <param name="argumentsToParse">
        /// The collection to search. 
        /// Specifying null will default to <see cref="Arguments"/>.
        /// </param>
        /// <returns>
        /// Searches <paramref name="argumentsToParse"/> for <paramref name="callChain"/>. 
        /// Once found, proceeding values are returned until a value begins with a known <see cref="ICommandLineSpecification.Delimiter"/>.
        /// 
        /// If <paramref name="callChain"/> is not found, or no parameters are found, an empty collection is returned.
        /// </returns>
        public static IEnumerable<string> GetParams(this IEnumerable<ICommandLineArgument> callChain, StringComparison? comparer = null, IEnumerable<string> argumentsToParse = null)
            => callChain.LastOrDefault().GetParams(comparer, callChain.GetSegment(comparer, argumentsToParse));
        #endregion

        #region ExcludeParams
        /// <summary>
        /// Returns the given collection, minus parameters.
        /// </summary>
        /// <param name="segment">The collection of arguments.</param>
        /// <param name="knownSpecifications">The collection of known specifications.</param>
        /// <returns>
        /// Returns the given collection, minus any values that do not begin with any 
        /// known <see cref="ICommandLineSpecification"/> delimiters.
        /// </returns>
        public static IEnumerable<string> ExcludeParams(IEnumerable<string> segment, IEnumerable<ICommandLineSpecification> knownSpecifications) => segment.Where(command => knownSpecifications.Select(icls => icls.Delimiter).Any(delim => command.StartsWith(delim)));

        /// <summary>
        /// Returns the given collection, minus parameters.
        /// </summary>
        /// <param name="segment">The collection of arguments.</param>
        /// <param name="knownSpecifications">The collection of known specifications.</param>
        /// <returns>
        /// Returns the given collection, minus any values that do not begin with any 
        /// known <see cref="ICommandLineSpecification"/> delimiters.
        /// </returns>
        public static IEnumerable<string> ExcludeParams(this IEnumerable<ICommandLineArgument> segment, IEnumerable<ICommandLineSpecification> knownSpecifications) => ExcludeParams(segment.Select(icla => icla.Command), knownSpecifications);
        #endregion

        #region Found
        /// <summary>
        /// Determines if the specified argument is present.
        /// </summary>
        /// <param name="rootArgument">The <see cref="ICommandLineArgument"/> to check.</param>
        /// <param name="options">The flags which dictate what determines an argument as found.</param>
        /// <param name="comparer">The comparer to use when determining if a match was found.</param>
        /// <param name="argumentsToParse">
        /// The collection to search. 
        /// Specifying null will default to <see cref="Arguments"/>.
        /// </param>
        /// <returns>
        /// Returns true if the apecified argument was found using the supplied flags; otherwise, returns false.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// Throws when an invalid flag combination is given.
        /// </exception>
        public static bool Found(this ICommandLineArgument rootArgument, CmdLineSearchOptions options, StringComparison? comparer = null, IEnumerable<string> argumentsToParse = null)
        {
            if (options.HasFlag(CmdLineSearchOptions.WithChildren | CmdLineSearchOptions.WithoutChildren))
                throw new ArgumentException($"Invalid flag combination. {options}", nameof(options));

            if (options.HasFlag(CmdLineSearchOptions.WithSiblings | CmdLineSearchOptions.WithoutSiblings))
                throw new ArgumentException($"Invalid flag combination. {options}", nameof(options));

            if (options.HasFlag(CmdLineSearchOptions.WithParams | CmdLineSearchOptions.WithoutParams))
                throw new ArgumentException($"Invalid flag combination. {options}", nameof(options));

            var segment = rootArgument.GetSegment(comparer, argumentsToParse);

            if (options == CmdLineSearchOptions.None)
                return segment.Any();

            if (!segment.Any())
                return false;

            if (options == CmdLineSearchOptions.WithChildren)
                return segment.Count() > 1;

            if (options == CmdLineSearchOptions.WithoutChildren)
                return segment.Count() == 1;

            if (options == CmdLineSearchOptions.WithParams)
                return segment.Any() && rootArgument.GetParams().Any();

            if (options == CmdLineSearchOptions.WithoutParams)
                return segment.Any() && !rootArgument.GetParams().Any();

            if (options == (CmdLineSearchOptions.WithChildren | CmdLineSearchOptions.WithParams))
                return segment.Count() > 1 && rootArgument.GetParams().Any();

            if (options == (CmdLineSearchOptions.WithChildren | CmdLineSearchOptions.WithoutParams))
                return segment.Count() > 1 && !rootArgument.GetParams().Any();

            if (options == (CmdLineSearchOptions.WithoutChildren | CmdLineSearchOptions.WithParams))
                return segment.Count() == 1 && rootArgument.GetParams().Any();

            if (options == (CmdLineSearchOptions.WithoutChildren | CmdLineSearchOptions.WithoutParams))
                return segment.Count() == 1 && !rootArgument.GetParams().Any();

            var hasSiblings = (argumentsToParse ?? Arguments).Any(str =>
                str.StartsWith(rootArgument.Specification.Delimiter) && !rootArgument.Command.Equals(str, (comparer ?? Comparer)));

            if (options == CmdLineSearchOptions.WithSiblings)
                return segment.Any() && hasSiblings;

            if (options == CmdLineSearchOptions.WithoutSiblings)
                return segment.Any() && !hasSiblings;

            if (options == (CmdLineSearchOptions.WithoutChildren | CmdLineSearchOptions.WithoutSiblings))
                return segment.Count() == 1 && !hasSiblings;

            if (options == (CmdLineSearchOptions.WithChildren | CmdLineSearchOptions.WithoutSiblings))
                return segment.Count() > 1 && !hasSiblings;

            if (options == (CmdLineSearchOptions.WithoutChildren | CmdLineSearchOptions.WithSiblings))
                return segment.Count() == 1 && hasSiblings;

            if (options == (CmdLineSearchOptions.WithChildren | CmdLineSearchOptions.WithSiblings))
                return segment.Count() > 1 && hasSiblings;

            if (options == (CmdLineSearchOptions.WithoutChildren | CmdLineSearchOptions.WithoutSiblings | CmdLineSearchOptions.WithoutParams))
                return segment.Count() == 1 && !hasSiblings && !rootArgument.GetParams().Any();

            if (options == (CmdLineSearchOptions.WithChildren | CmdLineSearchOptions.WithoutSiblings | CmdLineSearchOptions.WithoutParams))
                return segment.Count() > 1 && !hasSiblings && !rootArgument.GetParams().Any();

            if (options == (CmdLineSearchOptions.WithoutChildren | CmdLineSearchOptions.WithSiblings | CmdLineSearchOptions.WithoutParams))
                return segment.Count() == 1 && hasSiblings && !rootArgument.GetParams().Any();

            if (options == (CmdLineSearchOptions.WithChildren | CmdLineSearchOptions.WithSiblings | CmdLineSearchOptions.WithoutParams))
                return segment.Count() > 1 && hasSiblings && !rootArgument.GetParams().Any();

            if (options == (CmdLineSearchOptions.WithoutChildren | CmdLineSearchOptions.WithoutSiblings | CmdLineSearchOptions.WithParams))
                return segment.Count() == 1 && !hasSiblings && rootArgument.GetParams().Any();

            if (options == (CmdLineSearchOptions.WithChildren | CmdLineSearchOptions.WithoutSiblings | CmdLineSearchOptions.WithParams))
                return segment.Count() > 1 && !hasSiblings && rootArgument.GetParams().Any();

            if (options == (CmdLineSearchOptions.WithoutChildren | CmdLineSearchOptions.WithSiblings | CmdLineSearchOptions.WithParams))
                return segment.Count() == 1 && hasSiblings && rootArgument.GetParams().Any();

            if (options == (CmdLineSearchOptions.WithChildren | CmdLineSearchOptions.WithSiblings | CmdLineSearchOptions.WithParams))
                return segment.Count() > 1 && hasSiblings && rootArgument.GetParams().Any();

            throw new ArgumentException("Could not find a search match pattern using the supplied search options.", nameof(options));
        }

        /// <summary>
        /// Determines if the specified call-chain is present.
        /// </summary>
        /// <param name="callChain">The call-chain to check.</param>
        /// <param name="options">The flags which dictate what determines an argument as found.</param>
        /// <param name="comparer">The comparer to use when determining if a match was found.</param>
        /// <param name="argumentsToParse">
        /// The collection to search. 
        /// Specifying null will default to <see cref="Arguments"/>.
        /// </param>
        /// <returns>
        /// Returns true if the apecified argument was found using the supplied flags; otherwise, returns false.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// Throws when an invalid flag combination is given.
        /// </exception>
        public static bool Found(this IEnumerable<ICommandLineArgument> callChain, CmdLineSearchOptions options, StringComparison? comparer = null, IEnumerable<string> argumentsToParse = null)
        {
            var workingArg = callChain.Last();
            var parentCallChain = callChain.Take(callChain.Count() - 1);

            return workingArg.Found(options, comparer, parentCallChain.GetSegment(comparer, argumentsToParse));
        }

        /// <summary>
        /// Determines if the specified grouping is present.
        /// </summary>
        /// <param name="grouping">The grouping to check.</param>
        /// <param name="options">The flags which dictate what determines an argument as found.</param>
        /// <param name="comparer">The comparer to use when determining if a match was found.</param>
        /// <param name="argumentsToParse">
        /// The collection to search. 
        /// Specifying null will default to <see cref="Arguments"/>.
        /// </param>
        /// <returns>
        /// Returns true if the apecified argument was found using the supplied flags; otherwise, returns false.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// Throws when an invalid flag combination is given.
        /// </exception>
        public static bool Found(this ICommandLineGrouping grouping, CmdLineSearchOptions options, StringComparison? comparer = null, IEnumerable<string> argumentsToParse = null) => grouping.ParentCallChain.Concat(grouping.Children).Found(options, comparer, argumentsToParse);
        #endregion
    }
}