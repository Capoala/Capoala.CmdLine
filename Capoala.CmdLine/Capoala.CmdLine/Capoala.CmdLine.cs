using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Capoala.CmdLine
{
    /// <summary>
    /// Represents the characteristics of a command line argument.
    /// </summary>
    /// <remarks>
    /// The <see cref="ICommandLineSpecification"/> interface defines the characteristics of a command line argument, such as its delimiter and hierarchal order.
    /// </remarks>
    public interface ICommandLineSpecification : IEquatable<ICommandLineSpecification>, IComparable<ICommandLineSpecification>
    {
        /// <summary>
        /// Represents the hierarchal position in a parent-child relationship.
        /// Lower numbers represent a parent to a higher numbered child.
        /// </summary>
        int Hierarchy { get; }

        /// <summary>
        /// Represent the delimiter used for a command (switch).
        /// </summary>
        char Delimiter { get; }
    }

    /// <summary>
    /// Represents a command line argument.
    /// </summary>
    public interface ICommandLineArgument : IEquatable<ICommandLineArgument>, IComparable<ICommandLineArgument>
    {
        /// <summary>
        /// The command - switch - value used at the command line.
        /// <para>
        /// This property will always return the <see cref="Specification"/>.Delimiter.
        /// </para>
        /// </summary>
        string Command { get; }

        /// <summary>
        /// A description of what this argument represents or does within the scope of the application.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// The <see cref="ICommandLineSpecification"/> used to define this argument's characteristics.
        /// </summary>
        ICommandLineSpecification Specification { get; }
    }

    /// <summary>
    /// Represents a paired grouping between a parent chain and its associated child arguments.
    /// <para>
    /// *See remarks for further details and correct implementation.
    /// </para>
    /// </summary>
    /// <remarks>
    /// The <see cref="ICommandLineGrouping.ParentCallChain"/> property represents the full parent hierarchy chain. 
    /// 
    /// The <see cref="ICommandLineGrouping.Children"/> property represents associated child pairings. 
    /// It is important to note this property is not an "all inclusive" listing", but rather an individual grouping. 
    /// This means that if you have one "main argument", which has three "child arguments", then specifying a grouping
    /// with "main" and "childOne" would transpose to the command line as "$ main childOne". 
    /// 
    /// If you wanted to specify a grouping that states "main" must run with "childOne" and "childTwo", but leave "childThree" as optional, 
    /// then you would create two groupings; one with "childOne" and "childTwo"; the other, with "childOne", "childTwo", and "childThree".
    /// </remarks>
    public interface ICommandLineGrouping : IEquatable<ICommandLineGrouping>
    {
        /// <summary>
        /// The parent call-chain.
        /// </summary>
        ICommandLineArgument[] ParentCallChain { get; }

        /// <summary>
        /// The associated children in relation to the <see cref="ParentCallChain"/>.
        /// </summary>
        ICommandLineArgument[] Children { get; }
        /// <summary>
        /// A description of what this grouping represents or does within the scope of the application.
        /// </summary>
        string Description { get; set; }
    }

    /// <summary>
    /// Represents a command line restriction.
    /// </summary>
    /// <typeparam name="T">The object type to return should a violation occur.</typeparam>
    public interface ICommandLineRestriction<T>
    {
        /// <summary>
        /// Determines if a violation has occurred.
        /// </summary>
        bool IsViolated { get; }

        /// <summary>
        /// Performs all necessary queries to determine whether any violations were found, 
        /// returning all violation objects as <typeparamref name="T"/>.
        /// </summary>
        /// <returns>
        /// Returns a collection of <typeparamref name="T"/> representing the violations, if any violations where found.
        /// Returns an empty collection if no violations were found.
        /// </returns>
        IEnumerable<T> GetViolations();
    }

    /// <summary>
    /// Defines how a command line call-chain is found.
    /// </summary>
    public enum CmdLineSearchCriteria
    {
        /// <summary>
        /// All arguments of the call-chain must be found.
        /// </summary>
        Precise,
        /// <summary>
        /// Ignore children in the search.
        /// </summary>
        IgnoreChildren
    }

    /// <summary>
    /// Represents a violation.
    /// </summary>
    public struct CommandLineViolation
    {
        /// <summary>
        /// The violation.
        /// </summary>
        public string Violation { get; set; }

        /// <summary>
        /// The message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Creates a new <see cref="CommandLineViolation"/>.
        /// </summary>
        /// <param name="violation">The violation.</param>
        /// <param name="message">The message.</param>
        public CommandLineViolation(string violation, string message)
        {
            Violation = violation;
            Message = message;
        }
    }

    /// <summary>
    /// A default implementation of <see cref="ICommandLineSpecification"/>. This class cannot be inherited.
    /// <para>
    /// To implement a new <see cref="ICommandLineSpecification"/> implementation, 
    /// inherit from <see cref="CommandLine.BaseImplementations.CommandLineSpecificationBase"/>.
    /// </para>
    /// </summary>
    public sealed class CommandLineSpecification : CommandLine.BaseImplementations.CommandLineSpecificationBase
    {
        /// <summary>
        /// Creates new instance of <see cref="CommandLineSpecification"/>.
        /// </summary>
        /// <param name="hierarchy">The hierarchal position in a parent-child relationship.</param>
        /// <param name="delimiter">The delimiter used for a command (switch).</param>
        /// <exception cref="System.ArgumentException">
        /// Throws when <paramref name="delimiter"/> is not supported.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Throws when this could not be added to <see cref="CommandLine._knownSpecifications"/>.
        /// </exception>
        public CommandLineSpecification(int hierarchy, char delimiter)
            : base(hierarchy, delimiter) { }
    }

    /// <summary>
    /// A default implementation of <see cref="ICommandLineArgument"/>. This class cannot be inherited.
    /// <para>
    /// To implement a new <see cref="ICommandLineArgument"/> implementation, 
    /// inherit from <see cref="CommandLine.BaseImplementations.CommandLineArgumentBase"/>.
    /// </para>
    /// </summary>
    public sealed class CommandLineArgument : CommandLine.BaseImplementations.CommandLineArgumentBase
    {
        /// <summary>
        /// Creates new instance of <see cref="CommandLineArgument"/>.
        /// </summary>
        /// <param name="command">The command - switch - value used at the command line.</param>
        /// <param name="specification">The <see cref="ICommandLineSpecification"/> used to define this argument's characteristics.</param>
        /// <param name="description">A description of what this argument represents or does within the scope of the application.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Throws when <paramref name="command"/> is null, empty, or only whitespace.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Throws when <paramref name="specification"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Throws when this could not be added to <see cref="CommandLine._knownArguments"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Throws when <paramref name="specification"/> could not be added to <see cref="CommandLine._knownSpecifications"/>.
        /// </exception>
        public CommandLineArgument(string command, ICommandLineSpecification specification, string description = null)
            : base(command, specification, description) { }
    }

    /// <summary>
    /// A default implementation of <see cref="ICommandLineGrouping"/>. This class cannot be inherited.
    /// <para>
    /// To implement a new <see cref="ICommandLineGrouping"/> implementation, 
    /// inherit from <see cref="CommandLine.BaseImplementations.CommandLineGroupingBase"/>.
    /// </para>
    /// </summary>
    public sealed class CommandLineGrouping : CommandLine.BaseImplementations.CommandLineGroupingBase
    {
        /// <summary>
        /// Creates new instance of <see cref="CommandLineGrouping"/>.
        /// </summary>
        /// <param name="parentCallChain">The parent call-chain.</param>
        /// <param name="children">The associated children in relation to <paramref name="parentCallChain"/>.</param>
        /// <param name="description">A description of what this grouping represents or does within the scope of the application.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Throws when <paramref name="parentCallChain"/> is null or contains not elements.
        /// </exception>
        public CommandLineGrouping(ICommandLineArgument parentCallChain, ICommandLineArgument[] children, string description = null)
            : base(parentCallChain, children, description) { }

        /// <summary>
        /// Creates new instance of <see cref="CommandLineGrouping"/>.
        /// </summary>
        /// <param name="parentCallChain">The parent call-chain.</param>
        /// <param name="children">The associated children in relation to <paramref name="parentCallChain"/>.</param>
        /// <param name="description">A description of what this grouping represents or does within the scope of the application.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Throws when <paramref name="parentCallChain"/> is null or contains not elements.
        /// </exception>
        public CommandLineGrouping(ICommandLineArgument[] parentCallChain, ICommandLineArgument[] children, string description = null)
            : base(parentCallChain, children, description) { }
    }

    /// <summary>
    /// Provides the means to create, retrieve, and configure command line arguments. This class cannot be inherited.
    /// </summary>
    public static class CommandLine
    {
        /// <summary>
        /// A collection of all instantiated <see cref="ICommandLineArgument"/> implementations.
        /// </summary>
        private static HashSet<ICommandLineArgument> _knownArguments = new HashSet<ICommandLineArgument>();

        /// <summary>
        /// A collection of all instantiated <see cref="ICommandLineSpecification"/> implementations.
        /// </summary>
        private static HashSet<ICommandLineSpecification> _knownSpecifications = new HashSet<ICommandLineSpecification>();


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
        /// A collection of all instantiated <see cref="ICommandLineArgument"/> implementations.
        /// </summary>
        public static IEnumerable<ICommandLineArgument> KnownArguments => _knownArguments.AsEnumerable();

        /// <summary>
        /// A collection of all instantiated <see cref="ICommandLineSpecification"/> implementations.
        /// </summary>
        public static IEnumerable<ICommandLineSpecification> KnownSpecifications => _knownSpecifications.AsEnumerable();

        /// <summary>
        /// Returns the lowest <see cref="ICommandLineSpecification.Hierarchy"/> from the <see cref="KnownSpecifications"/> collection.
        /// </summary>
        public static int RootHierarchy => KnownSpecifications.Select(kspec => kspec.Hierarchy).Min();

        /// <summary>
        /// Returns the <see cref="ICommandLineSpecification"/> with the lowest <see cref="ICommandLineSpecification.Hierarchy"/> 
        /// from the <see cref="KnownSpecifications"/> collection.
        /// </summary>
        public static ICommandLineSpecification RootSpecification => KnownSpecifications.Where(ks => ks.Hierarchy == RootHierarchy).FirstOrDefault();


        /// <summary>
        /// Returns a collection of values starting at <paramref name="argument"/>, and ending at the next value that begins 
        /// with the first character of <paramref name="argument"/>.
        /// </summary>
        /// <param name="argument">The argument to search for.</param>
        /// <param name="collectionToSearch">
        /// The collection to search. 
        /// Specifying null will default to <see cref="CommandLine.Arguments"/>.
        /// </param>
        /// <returns>
        /// Returns all child <see cref="ICommandLineArgument.Command"/> and parameters based upon the 
        /// <see cref="ICommandLineSpecification"/> if <paramref name="argument"/> is found.
        /// Returns an empty collection if <paramref name="argument"/> is not found.
        /// </returns>
        /// <remarks>
        /// This method searches <paramref name="collectionToSearch"/> for a value that matches <paramref name="argument"/> 
        /// using the <see cref="CommandLine.Comparer"/> value.
        /// Once found, that value and all proceeding values are returned until a value whose first character matches that 
        /// of the <paramref name="argument"/>'s delimiter.
        /// 
        /// If <paramref name="collectionToSearch"/> is null, <see cref="CommandLine.Arguments"/> is used.
        /// 
        /// If no matches to <paramref name="argument"/> are found, an empty collection is returned.
        /// </remarks>
        public static IEnumerable<string> GetSegment(this ICommandLineArgument argument, IEnumerable<string> collectionToSearch = null)
        {
            collectionToSearch = collectionToSearch ?? Arguments;
            if (collectionToSearch.Any())
            {
                IEnumerable<string> remaining = collectionToSearch.SkipWhile(str => !str.Equals(argument.Command, Comparer));
                if (remaining.Any())
                {
                    yield return remaining.FirstOrDefault();
                    foreach (string command in remaining.Skip(1).TakeWhile(str => !str.FirstOrDefault().Equals(argument.Specification.Delimiter)))
                        yield return command;
                }
            }
        }

        /// <summary>
        /// Returns a collection of child arguments and parameters of the given call-chain.
        /// </summary>
        /// <param name="callChain">The call-chain.</param>
        /// <param name="collectionToSearch">
        /// The collection to search. 
        /// Specifying null will default to <see cref="CommandLine.Arguments"/>.
        /// </param>
        /// <returns>
        /// Returns all child <see cref="ICommandLineArgument.Command"/> and parameters based upon the 
        /// <see cref="ICommandLineSpecification"/> if <paramref name="callChain"/> is found.
        /// Returns an empty collection if <paramref name="callChain"/> is not found.
        /// </returns>
        /// <remarks>
        /// This method searches <paramref name="collectionToSearch"/> for a value that matches <paramref name="callChain"/> 
        /// using the <see cref="CommandLine.Comparer"/> value.
        /// Once found, that value and all proceeding values are returned until a value whose first character matches that 
        /// of the <paramref name="callChain"/>'s last argument's delimiter.
        /// 
        /// If <paramref name="collectionToSearch"/> is null, <see cref="CommandLine.Arguments"/> is used.
        /// 
        /// If no matches to <paramref name="callChain"/> are found, an empty collection is returned.
        /// </remarks>
        public static IEnumerable<string> GetSegment(this IEnumerable<ICommandLineArgument> callChain, IEnumerable<string> collectionToSearch = null)
        {
            var workingSegment = collectionToSearch ?? Arguments;

            if (workingSegment.Any())
            {
                foreach (ICommandLineArgument icla in callChain)
                    workingSegment = GetSegment(icla, workingSegment);
            }

            return workingSegment;
        }

        /// <summary>
        /// Returns a collection of child arguments and parameters of the given call-chain.
        /// </summary>
        /// <param name="callChain">The call-chain.</param>
        /// <returns>
        /// Returns all child <see cref="ICommandLineArgument.Command"/> and parameters based upon the 
        /// <see cref="ICommandLineSpecification"/> if <paramref name="callChain"/> is found.
        /// Returns an empty collection if <paramref name="callChain"/> is not found.
        /// </returns>
        /// <remarks>
        /// This method searches <see cref="CommandLine.Arguments"/> for a value that matches <paramref name="callChain"/> 
        /// using the <see cref="CommandLine.Comparer"/> value.
        /// Once found, that value and all proceeding values are returned until a value whose first character matches that 
        /// of the <paramref name="callChain"/>'s last argument's delimiter.
        /// 
        /// If no matches to <paramref name="callChain"/> are found, an empty collection is returned.
        /// </remarks>
        public static IEnumerable<string> GetSegment(params ICommandLineArgument[] callChain)
            => callChain.AsEnumerable().GetSegment();


        /// <summary>
        /// Returns all parameters associated with the specified <see cref="ICommandLineArgument"/>.
        /// </summary>
        /// <param name="argument">The argument to retrieve the parameters of.</param>
        /// <param name="collectionToSearch">
        /// The collection to search. 
        /// Specifying null will default to <see cref="CommandLine.Arguments"/>.
        /// </param>
        /// <returns>
        /// Searches <paramref name="collectionToSearch"/> for <paramref name="argument"/>. 
        /// Once found, proceeding values are returned until a value begins with a known <see cref="ICommandLineSpecification.Delimiter"/>.
        /// 
        /// If <paramref name="argument"/> is not found, or no parameters are found, an empty collection is returned.
        /// </returns>
        public static IEnumerable<string> GetParams(this ICommandLineArgument argument, IEnumerable<string> collectionToSearch = null)
            => argument.GetSegment(collectionToSearch ?? Arguments)
                       .Skip(1)
                       .TakeWhile(arg => !KnownSpecifications.Select(ks => ks.Delimiter)
                                                             .Contains(arg.FirstOrDefault()));

        /// <summary>
        /// Returns all parameters associated with the specified call-chain.
        /// </summary>
        /// <param name="callChain">The call-chain to retrieve the parameters of.</param>
        /// <param name="collectionToSearch">
        /// The collection to search. 
        /// Specifying null will default to <see cref="CommandLine.Arguments"/>.
        /// </param>
        /// <returns>
        /// Searches <paramref name="collectionToSearch"/> for <paramref name="callChain"/>. 
        /// Once found, proceeding values are returned until a value begins with a known <see cref="ICommandLineSpecification.Delimiter"/>.
        /// 
        /// If <paramref name="callChain"/> is not found, or no parameters are found, an empty collection is returned.
        /// </returns>
        public static IEnumerable<string> GetParams(this ICommandLineArgument[] callChain, IEnumerable<string> collectionToSearch = null)
            => callChain.LastOrDefault().GetParams(callChain.FirstOrDefault().GetSegment(collectionToSearch ?? Arguments));

        /// <summary>
        /// Returns all parameters associated with the specified call-chain.
        /// </summary>
        /// <param name="callChain">The call-chain to retrieve the parameters of.</param>
        /// <returns>
        /// Searches <see cref="CommandLine.Arguments"/> for <paramref name="callChain"/>. 
        /// Once found, proceeding values are returned until a value begins with a known <see cref="ICommandLineSpecification.Delimiter"/>.
        /// 
        /// If <paramref name="callChain"/> is not found, or no parameters are found, an empty collection is returned.
        /// </returns>
        public static IEnumerable<string> GetParams(params ICommandLineArgument[] callChain)
            => callChain.GetParams();


        /// <summary>
        /// Returns the given collection, minus parameters.
        /// </summary>
        /// <param name="collectionToFilter">The collection of arguments.</param>
        /// <returns>
        /// Returns the given collection, minus any values that do not begin with any 
        /// known <see cref="ICommandLineSpecification"/> delimiters.
        /// </returns>
        public static IEnumerable<string> ExcludeParams(IEnumerable<string> collectionToFilter)
            => collectionToFilter.Where(str => KnownSpecifications.Select(icls => icls.Delimiter).Contains(str.FirstOrDefault()));

        /// <summary>
        /// Returns the given collection, minus parameters.
        /// </summary>
        /// <param name="collectionToFilter">The collection of arguments.</param>
        /// <returns>
        /// Returns the given collection, minus any values that do not begin with any 
        /// known <see cref="ICommandLineSpecification"/> delimiters.
        /// </returns>
        public static IEnumerable<string> ExcludeParams(IEnumerable<ICommandLineArgument> collectionToFilter)
            => ExcludeParams(collectionToFilter.Select(icla => icla.Command));


        /// <summary>
        /// Determines if the specified call-chain is present in the <see cref="CommandLine.Arguments"/> collection.
        /// <para>
        /// This overload takes proceeding, children arguments into consideration.
        /// </para>
        /// </summary>
        /// <param name="callChain">The call-chain to check.</param>
        /// <returns>
        /// Returns true if the call-chain is present; otherwise, returns false.
        /// </returns>
        public static bool Found(params ICommandLineArgument[] callChain)
            => callChain.AsEnumerable().Found();

        /// <summary>
        /// Determines if the specified call-chain is present in the specified collection.
        /// </summary>
        /// <param name="callChain">The call-chain to check.</param>
        /// <param name="criteria">Determines whether children proceeding <paramref name="callChain"/> should be included as part of the match.</param>
        /// <param name="collectionToSearch">
        /// The collection to search. 
        /// Specifying null will default to <see cref="CommandLine.Arguments"/>.
        /// </param>
        /// <returns>
        /// Returns true if the call-chain is present; otherwise, returns false.
        /// </returns>
        public static bool Found(this IEnumerable<ICommandLineArgument> callChain, CmdLineSearchCriteria criteria = CmdLineSearchCriteria.Precise, IEnumerable<string> collectionToSearch = null)
        {
            collectionToSearch = ExcludeParams(collectionToSearch ?? Arguments);
            if (collectionToSearch.Any())
            {
                var commands = callChain.Select(icla => icla.Command);
                int countOfRoot = collectionToSearch.Count(str => callChain.First().Command.Equals(str, Comparer));

                if (criteria == CmdLineSearchCriteria.IgnoreChildren)
                {
                    bool mismatch = false;
                    for (int i = 0; i < countOfRoot; i++)
                    {
                        var segment = callChain.First().GetSegment(collectionToSearch);

                        if (commands.Count() < segment.Count())
                        {
                            for (int cmd = 0; cmd < commands.Count(); cmd++)
                            {
                                if (!commands.ElementAt(cmd).Equals(segment.ElementAt(cmd), Comparer))
                                {
                                    mismatch = true;
                                    break;
                                }
                            }
                            if (mismatch)
                                collectionToSearch = collectionToSearch.ExcludeSubset(segment);
                        }
                    }
                    return !mismatch;
                }
                else
                {
                    for (int i = 0; i < countOfRoot; i++)
                    {
                        var segment = callChain.First().GetSegment(collectionToSearch);

                        if (segment.UnorderedSequenceEquals(commands))
                            return true;
                        else
                            collectionToSearch = collectionToSearch.ExcludeSubset(segment);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if the specified <see cref="ICommandLineGrouping"/> is present in the specified collection.
        /// </summary>
        /// <param name="grouping">The <see cref="ICommandLineGrouping"/> to evaluate.</param>
        /// <param name="collectionToSearch">
        /// The collection to search. 
        /// Specifying null will default to <see cref="CommandLine.Arguments"/>.
        /// </param>
        /// <returns>
        /// Returns true if the call-chain is present; otherwise, returns false.
        /// </returns>
        public static bool Found(this ICommandLineGrouping grouping, IEnumerable<string> collectionToSearch = null)
            => grouping.ParentCallChain.Concat(grouping.Children).Found( CmdLineSearchCriteria.Precise, collectionToSearch);

        // TODO: Implement
        ///// <summary>
        ///// Returns a listing of all possible command line combinations.
        ///// </summary>
        ///// <returns></returns>
        //public static IEnumerable<IEnumerable<string>> AllPossibleCombinations()
        //{
        //    var minSpec = KnownSpecifications.Min(spec => spec.Hierarchy);
        //    var maxSpec = KnownSpecifications.Max(spec => spec.Hierarchy);

        //    var stack = new Stack<List<string>>();
        //    var lastTierPairwiseCombinations = KnownArguments.Where(ka => ka.Specification.Hierarchy == maxSpec).Select(ka => ka.Command).PairwiseCombinations();
        //    var fullBuild = Enumerable.Empty<IEnumerable<string>>();

        //    for (int i = minSpec; i <= maxSpec - 1; i++)
        //    {
        //        var item = KnownArguments.Where(ka => ka.Specification.Hierarchy == i).Select(ka => ka.Command);
        //        stack.Push(item.ToList());
        //    }

        //    fullBuild = stack.Pop().Build(lastTierPairwiseCombinations);

        //    for (int i = 0; i <= stack.Count; i++)
        //    {
        //        fullBuild = stack.Pop().Build(fullBuild);
        //    }

        //    foreach (var pairwise in fullBuild.PairwiseSpecial())
        //    {
        //        yield return pairwise;
        //    }
        //}


        /// <summary>
        /// Contains base interface implementations. This class cannot be inherited.
        /// </summary>
        public static class BaseImplementations
        {
            /// <summary>
            /// The base implementation of <see cref="ICommandLineSpecification"/>. This class can only be inherited.
            /// </summary>
            public abstract class CommandLineSpecificationBase : ICommandLineSpecification
            {
                /// <summary>
                /// Represents the hierarchal position in a parent-child relationship.
                /// Lower numbers represent a parent to a higher numbered child.
                /// </summary>
                public int Hierarchy { get; }

                /// <summary>
                /// Represent the delimiter used for a command (switch).
                /// </summary>
                public char Delimiter { get; }

                /// <summary>
                /// Creates new instance of <see cref="CommandLineSpecificationBase"/>.
                /// </summary>
                /// <param name="hierarchy">The hierarchal position in a parent-child relationship.</param>
                /// <param name="delimiter">The delimiter used for a command (switch).</param>
                /// <exception cref="System.ArgumentException">
                /// Throws when <paramref name="delimiter"/> is not supported.
                /// </exception>
                /// <exception cref="System.ArgumentException">
                /// Throws when this could not be added to <see cref="CommandLine._knownSpecifications"/>.
                /// </exception>
                protected CommandLineSpecificationBase(int hierarchy, char delimiter)
                {
                    if (char.IsWhiteSpace(delimiter) ||
                        char.IsLetterOrDigit(delimiter) ||
                        char.IsNumber(delimiter) ||
                        delimiter == '"' || delimiter == '\'')
                        throw new ArgumentException("The character is not supported.", "specification");

                    Hierarchy = hierarchy;
                    Delimiter = delimiter;

                    if (!_knownSpecifications.Add(this))
                        throw new ArgumentException("An object with the same value already exists.", "this");
                }

                /// <summary>
                /// Compares the current instance with another object of the same type and returns an integer 
                /// that indicates whether the current instance precedes, follows, or occurs in the same position 
                /// in the sort order as the other object.
                /// </summary>
                /// <param name="other">The <see cref="ICommandLineSpecification"/> to compare to.</param>
                /// <returns>
                /// Returns 0 if both instance occur in the same sort order.
                /// Returns 1 if <paramref name="other"/> is null or follows this instance.
                /// Returns greater than zero if this instance follows <paramref name="other"/> in the sort order.
                /// Returns less than zero if this instance precedes <paramref name="other"/> in the sort order.
                /// </returns>
                public int CompareTo(ICommandLineSpecification other)
                {
                    if (other == null)
                        return 1;

                    var result = Hierarchy.CompareTo(other.Hierarchy);

                    if (result == 0)
                        result = Delimiter.CompareTo(other.Delimiter);

                    return result;
                }

                /// <summary>
                /// Determines whether two specified <see cref="ICommandLineSpecification"/> have the same value.
                /// </summary>
                /// <param name="other">The <see cref="ICommandLineSpecification"/> to compare to this instance.</param>
                /// <returns>
                /// Returns true if both objects re the same instance or hold the same values; otherwise, returns false.
                /// </returns>
                public bool Equals(ICommandLineSpecification other)
                {
                    if (ReferenceEquals(other, null))
                        return false;
                    else if (ReferenceEquals(this, other))
                        return true;
                    else if (GetType() != other.GetType())
                        return false;
                    else
                        return (Hierarchy == other.Hierarchy) && (Delimiter == other.Delimiter);
                }

                /// <summary>
                /// Determines whether the specified object is equal to the current object.
                /// </summary>
                /// <param name="obj">The object to compare with the current object.</param>
                /// <returns>
                /// Returns true if both objects re the same instance or hold the same values; otherwise, returns false.
                /// </returns>
                public override bool Equals(object obj) => Equals(obj as ICommandLineSpecification);

                /// <summary>
                /// Serves as the default hash function. 
                /// </summary>
                /// <returns>
                /// A hash code for the current object.
                /// </returns>
                public override int GetHashCode()
                {
                    unchecked
                    {
                        int hash = 17;
                        const int multiplier = 23;
                        hash = hash * multiplier + Hierarchy.GetHashCode();
                        hash = hash * multiplier + Delimiter.GetHashCode();
                        return hash;
                    }
                }

                /// <summary>
                /// Returns a string that represents the current object.
                /// </summary>
                /// <returns>
                /// Returns the <see cref="ICommandLineSpecification.Delimiter"/> value.
                /// </returns>
                public override string ToString() => $"{Delimiter}";
            }

            /// <summary>
            /// The base implementation of <see cref="ICommandLineArgument"/>. This class can only be inherited.
            /// </summary>
            public abstract class CommandLineArgumentBase : ICommandLineArgument
            {
                /// <summary>
                /// The command - switch - value used at the command line.
                /// <para>
                /// This property will always return the <see cref="Specification"/>.Delimiter.
                /// </para>
                /// </summary>
                public string Command { get; }

                /// <summary>
                /// A description of what this argument represents or does within the scope of the application.
                /// </summary>
                public string Description { get; set; }

                /// <summary>
                /// TThe <see cref="ICommandLineSpecification"/> used to define this argument's characteristics.
                /// </summary>
                public ICommandLineSpecification Specification { get; }

                /// <summary>
                /// Creates new instance of <see cref="CommandLineArgumentBase"/>.
                /// </summary>
                /// <param name="command">The command - switch - value used at the command line.</param>
                /// <param name="specification">The <see cref="ICommandLineSpecification"/> used to define this argument's characteristics.</param>
                /// <param name="description">A description of what this argument represents or does within the scope of the application.</param>
                /// <exception cref="System.ArgumentNullException">
                /// Throws when <paramref name="command"/> is null, empty, or only whitespace.
                /// </exception>
                /// <exception cref="System.ArgumentNullException">
                /// Throws when <paramref name="specification"/> is null.
                /// </exception>
                /// <exception cref="System.ArgumentException">
                /// Throws when this could not be added to <see cref="CommandLine._knownArguments"/>.
                /// </exception>
                /// <exception cref="System.ArgumentException">
                /// Throws when <paramref name="specification"/> could not be added to <see cref="CommandLine._knownSpecifications"/>.
                /// </exception>
                protected CommandLineArgumentBase(string command, ICommandLineSpecification specification, string description = null)
                {
                    if (string.IsNullOrWhiteSpace(command))
                        throw new ArgumentNullException("command");

                    Specification = specification ?? throw new ArgumentNullException("specification");

                    Command = $"{specification.Delimiter}{command.TrimStart(specification.Delimiter)}";
                    Description = description;

                    if (!_knownArguments.Add(this))
                        throw new ArgumentException("An ICommandLineArgument object with the same value already exists.", "this");

                    if (!_knownSpecifications.Contains(specification))
                        if (!_knownSpecifications.Add(specification))
                            throw new ArgumentException("The ICommandLineSpecification object could not be added to the collection of known ICommandLineSpecifications.", "specification");
                }

                /// <summary>
                /// Compares the current instance with another object of the same type and returns an integer 
                /// that indicates whether the current instance precedes, follows, or occurs in the same position 
                /// in the sort order as the other object.
                /// </summary>
                /// <param name="other">The <see cref="ICommandLineArgument"/> to compare to.</param>
                /// <returns>
                /// Returns 0 if both instance occur in the same sort order.
                /// Returns 1 if <paramref name="other"/> is null or follows this instance.
                /// Returns greater than zero if this instance follows <paramref name="other"/> in the sort order.
                /// Returns less than zero if this instance precedes <paramref name="other"/> in the sort order.
                /// </returns>
                public int CompareTo(ICommandLineArgument other)
                {
                    if (other == null)
                        return 1;

                    var result = Command.CompareTo(other.Command);

                    if (result == 0)
                        result = Specification.CompareTo(other.Specification);
                    if (result == 0)
                        result = string.Compare(Description, other.Description);
                    return result;
                }

                /// <summary>
                /// Determines whether two specified <see cref="ICommandLineArgument"/> have the same value.
                /// </summary>
                /// <param name="other">The <see cref="ICommandLineArgument"/> to compare to this instance.</param>
                /// <returns>
                /// Returns true if both objects re the same instance or hold the same values; otherwise, returns false.
                /// </returns>
                public bool Equals(ICommandLineArgument other)
                {
                    if (ReferenceEquals(other, null))
                        return false;
                    else if (ReferenceEquals(this, other))
                        return true;
                    else if (GetType() != other.GetType())
                        return false;
                    else
                        return (Command == other.Command) &&
                               (Specification == other.Specification) &&
                               (Description == other.Description);
                }

                /// <summary>
                /// Determines whether the specified object is equal to the current object.
                /// </summary>
                /// <param name="obj">The object to compare with the current object.</param>
                /// <returns>
                /// Returns true if both objects re the same instance or hold the same values; otherwise, returns false.
                /// </returns>
                public override bool Equals(object obj) => Equals(obj as ICommandLineArgument);

                /// <summary>
                /// Serves as the default hash function. 
                /// </summary>
                /// <returns>
                /// A hash code for the current object.
                /// </returns>
                public override int GetHashCode()
                {
                    unchecked
                    {
                        int hash = 17;
                        const int multiplier = 23;
                        hash = hash * multiplier + Command.GetHashCode();
                        hash = hash * multiplier + Specification.GetHashCode();
                        hash = hash * multiplier + Description?.GetHashCode() ?? 0;
                        return hash;
                    }
                }

                /// <summary>
                /// Returns a string that represents the current object.
                /// </summary>
                /// <returns>
                /// Returns the <see cref="ICommandLineArgument.Command"/> value.
                /// </returns>
                public override string ToString() => $"{Command}";
            }

            /// <summary>
            /// The base implementation of <see cref="ICommandLineGrouping"/>. This class can only be inherited.
            /// </summary>
            public abstract class CommandLineGroupingBase : ICommandLineGrouping
            {
                /// <summary>
                /// The parent call-chain.
                /// </summary>
                public ICommandLineArgument[] ParentCallChain { get; }

                /// <summary>
                /// The associated children in relation to <see cref="ParentCallChain"/>.
                /// </summary>
                public ICommandLineArgument[] Children { get; }

                /// <summary>
                /// A description of what this grouping represents or does within the scope of the application.
                /// </summary>
                public string Description { get; set; }

                /// <summary>
                /// Creates new instance of <see cref="CommandLineGroupingBase"/>.
                /// <para>
                /// *See remarks for further details and correct implementation.
                /// </para>
                /// </summary>
                /// <param name="parentCallChain">The parent call-chain.</param>
                /// <param name="children">The associated children in relation to <paramref name="parentCallChain"/>.</param>
                /// <param name="description">A description of what this grouping represents or does within the scope of the application.</param>
                /// <exception cref="System.ArgumentNullException">
                /// Throws when <paramref name="parentCallChain"/> is null or contains not elements.
                /// </exception>
                /// <remarks>
                /// The <see cref="ICommandLineGrouping.ParentCallChain"/> property represents the full parent hierarchy chain. 
                /// 
                /// The <see cref="ICommandLineGrouping.Children"/> property represents associated child pairings. 
                /// It is important to note this property is not an "all inclusive" listing", but rather an individual grouping. 
                /// This means that if you have one "main argument", which has three "child arguments", then specifying a grouping
                /// with "main" and "childOne" would transpose to the command line as "$ main childOne". 
                /// 
                /// If you wanted to specify a grouping that states "main" must run with "childOne" and "childTwo", but leave "childThree" as optional, 
                /// then you would create two groupings; one with "childOne" and "childTwo"; the other, with "childOne", "childTwo", and "childThree".
                /// </remarks>
                protected CommandLineGroupingBase(ICommandLineArgument parentCallChain, ICommandLineArgument[] children, string description = null)
                    : this(new ICommandLineArgument[] { parentCallChain }, children, description) { }

                /// <summary>
                /// Creates new instance of <see cref="CommandLineGroupingBase"/>.
                /// <para>
                /// *See remarks for further details and correct implementation.
                /// </para>
                /// </summary>
                /// <param name="parentCallChain">The parent call-chain.</param>
                /// <param name="children">The associated children in relation to <paramref name="parentCallChain"/>.</param>
                /// <param name="description">A description of what this grouping represents or does within the scope of the application.</param>
                /// <exception cref="System.ArgumentNullException">
                /// Throws when <paramref name="parentCallChain"/> is null or contains not elements.
                /// </exception>
                /// <remarks>
                /// The <see cref="ICommandLineGrouping.ParentCallChain"/> property represents the full parent hierarchy chain. 
                /// 
                /// The <see cref="ICommandLineGrouping.Children"/> property represents associated child pairings. 
                /// It is important to note this property is not an "all inclusive" listing", but rather an individual grouping. 
                /// This means that if you have one "main argument", which has three "child arguments", then specifying a grouping
                /// with "main" and "childOne" would transpose to the command line as "$ main childOne". 
                /// 
                /// If you wanted to specify a grouping that states "main" must run with "childOne" and "childTwo", but leave "childThree" as optional, 
                /// then you would create two groupings; one with "childOne" and "childTwo"; the other, with "childOne", "childTwo", and "childThree".
                /// </remarks>
                protected CommandLineGroupingBase(ICommandLineArgument[] parentCallChain, ICommandLineArgument[] children, string description = null)
                {
                    if (!parentCallChain?.Any() ?? false)
                        throw new ArgumentNullException("parentArgument");

                    ParentCallChain = parentCallChain;
                    Children = children ?? new ICommandLineArgument[] { };
                    Description = description;
                }

                /// <summary>
                /// Determines whether two specified <see cref="ICommandLineGrouping"/> have the same value.
                /// </summary>
                /// <param name="other">The <see cref="ICommandLineGrouping"/> to compare to this instance.</param>
                /// <returns>
                /// Returns true if both objects re the same instance or hold the same values; otherwise, returns false.
                /// </returns>
                public bool Equals(ICommandLineGrouping other)
                {
                    if (ReferenceEquals(other, null))
                        return false;
                    else if (ReferenceEquals(this, other))
                        return true;
                    else if (GetType() != other.GetType())
                        return false;
                    else
                        return ParentCallChain == other.ParentCallChain && Children == other.Children;
                }

                /// <summary>
                /// Determines whether the specified object is equal to the current object.
                /// </summary>
                /// <param name="obj">The object to compare with the current object.</param>
                /// <returns>
                /// Returns true if both objects re the same instance or hold the same values; otherwise, returns false.
                /// </returns>
                public override bool Equals(object obj) => Equals(obj as ICommandLineGrouping);

                /// <summary>
                /// Serves as the default hash function. 
                /// </summary>
                /// <returns>
                /// A hash code for the current object.
                /// </returns>
                public override int GetHashCode()
                {
                    unchecked
                    {
                        int hash = 17;
                        const int multiplier = 23;
                        hash = hash * multiplier + ParentCallChain.GetHashCode();
                        hash = hash * multiplier + Children?.GetHashCode() ?? 0;
                        return hash;
                    }
                }

                /// <summary>
                /// Returns a string that represents the current object.
                /// </summary>
                /// <returns>
                /// Returns the full string value of the call-chain and children.
                /// </returns>
                public override string ToString() => string.Join(" ", ParentCallChain.Select(icla => icla.Command).Concat(Children.Select(icla => icla.Command)));
            }

            /// <summary>
            /// The base implementation of <see cref="ICommandLineRestriction{T}"/>. This class can only be inherited.
            /// </summary>
            /// <typeparam name="T">The object type to return should a violation occur.</typeparam>
            public abstract class CommandLineRestrictionBase<T> : ICommandLineRestriction<T>
            {
                /// <summary>
                /// Determines if a violation has occurred.
                /// </summary>
                public virtual bool IsViolated => GetViolations().Any();

                /// <summary>
                /// Performs all necessary queries to determine whether any violations were found, returning all violation objects as <typeparamref name="T"/>.
                /// </summary>
                /// <returns>
                /// Returns a collection of <typeparamref name="T"/> representing the violations, if any violations where found.
                /// Returns an empty collection if no violations were found.
                /// </returns>
                public abstract IEnumerable<T> GetViolations();
            }
        }
    }

    /// <summary>
    /// Contains default, common, <see cref="ICommandLineRestriction{T}"/> implementations.
    /// </summary>
    public static class CommandLineRestrictions
    {
        /// <summary>
        /// A restriction which specifies legal arguments.
        /// </summary>
        public sealed class LegalArguments : CommandLine.BaseImplementations.CommandLineRestrictionBase<CommandLineViolation>
        {
            /// <summary>
            /// A collection of <see cref="ICommandLineGrouping"/> representing legal call-chains and associated child <see cref="ICommandLineArgument"/>.
            /// </summary>
            public ICommandLineGrouping[] LegalCombinations { get; }

            /// <summary>
            /// Creates a new instance of <see cref="LegalArguments"/>.
            /// </summary>
            /// <param name="legalCombinations">A collection of <see cref="ICommandLineGrouping"/> representing legal call-chains and associated child <see cref="ICommandLineArgument"/>.</param>
            public LegalArguments(params ICommandLineGrouping[] legalCombinations)
                => LegalCombinations = legalCombinations;

            /// <summary>
            /// Performs all necessary queries to determine whether any violations were found, returning all violation objects as <see cref="CommandLineViolation"/>.
            /// </summary>
            /// <returns>
            /// Returns a collection of <see cref="CommandLineViolation"/> representing the violations, if any violations where found.
            /// Returns an empty collection if no violations were found.
            /// </returns>
            public override IEnumerable<CommandLineViolation> GetViolations()
            {
                IEnumerable<string> rootArgs = CommandLine.Arguments.Where(str => str.FirstOrDefault().Equals(CommandLine.RootSpecification.Delimiter));
                IEnumerable<ICommandLineArgument> rootICLAs = CommandLine.KnownArguments.Where(icla => rootArgs.Contains(icla.Command));
                IEnumerable<string> workingCollection = CommandLine.ExcludeParams(CommandLine.Arguments);
                List<IEnumerable<string>> rootSegments = new List<IEnumerable<string>>();

                foreach (ICommandLineArgument rootArg in rootICLAs)
                {
                    var segment = CommandLine.GetSegment(rootArg, workingCollection);

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
                        yield return new CommandLineViolation("IllegalArgument", string.Join(" ", rootSegment));
            }
        }

        /// <summary>
        /// A restriction which restricts unknown arguments.
        /// </summary>
        public sealed class UnknownArgumentRestriction : CommandLine.BaseImplementations.CommandLineRestrictionBase<CommandLineViolation>
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
                if (CommandLine.Arguments.Any())
                {
                    var unknownArgs = CommandLine.ExcludeParams(CommandLine.Arguments).Except(CommandLine.KnownArguments.Select(icla => icla.Command));
                    if (unknownArgs.Any())
                        foreach (string unknownArg in unknownArgs)
                            yield return new CommandLineViolation("UnknownArgument", unknownArg);
                }
            }
        }

        /// <summary>
        /// A restriction which enforces the hierarchal order.
        /// </summary>
        public sealed class HierarchyRestriction : CommandLine.BaseImplementations.CommandLineRestrictionBase<CommandLineViolation>
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
                if (CommandLine.Arguments.Any())
                    if (CommandLine.Arguments.FirstOrDefault().FirstOrDefault() != CommandLine.RootSpecification.Delimiter)
                        yield return new CommandLineViolation("HierarchyViolation", CommandLine.Arguments.FirstOrDefault());
            }
        }

        /// <summary>
        /// A restriction which specifies how many parameters an <see cref="ICommandLineArgument"/> should have.
        /// </summary>
        public sealed class ParameterCountRestriction : CommandLine.BaseImplementations.CommandLineRestrictionBase<CommandLineViolation>
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
                if (CommandLine.Arguments.Any())
                {
                    if (CommandLine.Found(CallChain))
                    {
                        int paramCount = CommandLine.GetParams(CallChain).Count();
                        if (paramCount < MinParamsAllowed)
                            yield return new CommandLineViolation("TooFewParams", string.Join(" ", CallChain.Select(icla => icla.Command)));
                        else if (paramCount > MaxParamsAllowed)
                            yield return new CommandLineViolation("TooManyParams", string.Join(" ", CallChain.Select(icla => icla.Command)));
                    }
                }
            }
        }

        /// <summary>
        /// A restriction which specifies illegal combinations.
        /// </summary>
        public sealed class IllegalComboRestriction : CommandLine.BaseImplementations.CommandLineRestrictionBase<CommandLineViolation>
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
                    if (CommandLine.Found(FirstGrouping) && CommandLine.Found(SecondGrouping))
                        yield return new CommandLineViolation(
                            "IllegalCombo",
                            $"{string.Join(" ", firstChain.Select(icla => icla.Command))} and {string.Join(" ", secondChain.Select(icla => icla.Command))}");
                }
            }
        }

        /// <summary>
        /// A restriction which specifies mandated combinations.
        /// </summary>
        public sealed class MandatedComboRestriction : CommandLine.BaseImplementations.CommandLineRestrictionBase<CommandLineViolation>
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
                    bool firstPresent = CommandLine.Found(FirstGrouping);
                    bool secondPresent = CommandLine.Found(SecondGrouping);
                    if (firstPresent || secondPresent)
                        if (!(firstPresent && secondPresent))
                            yield return new CommandLineViolation(
                                "MandatedComboViolation",
                                $"{string.Join(" ", firstChain.Select(icla => icla.Command))} and {string.Join(" ", secondChain.Select(icla => icla.Command))}");
                }
            }
        }
    }

    /// <summary>
    /// Contains extensions methods that extend native LINQ functionality.
    /// </summary>
    internal static class CommandLineLinq
    {
        /// <summary>
        /// Returns the specified collection, minus the given subset. 
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="firstSequence">The source collection.</param>
        /// <param name="secondSequence">The subset to remove from the source collection.</param>
        /// <returns>
        /// Returns the source collection, minus the subset. 
        /// If the subset was not found, then the source collection is returned without modification.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Throws when <paramref name="firstSequence"/> is null.
        /// </exception>
        internal static IEnumerable<T> ExcludeSubset<T>(this IEnumerable<T> firstSequence, IEnumerable<T> secondSequence)
        {
            if (firstSequence == null)
                throw new ArgumentNullException("sourceCollection");

            if (secondSequence?.Any() ?? false)
            {
                if (secondSequence.Count() <= firstSequence.Count())
                {
                    int indexOfFirstMatch = firstSequence.ToList().IndexOf(secondSequence.FirstOrDefault());

                    if (indexOfFirstMatch != -1)
                    {
                        IEnumerable<T> remaining = firstSequence.Skip(indexOfFirstMatch);

                        if (remaining.Any())
                        {
                            bool mismatchFound = false;
                            for (int i = 0; i < secondSequence.Count(); i++)
                            {
                                T sourceCurrentElement = firstSequence.ElementAt(indexOfFirstMatch + i);
                                T subsetCurrentElement = secondSequence.ElementAt(i);

                                if (!sourceCurrentElement.Equals(subsetCurrentElement))
                                {
                                    mismatchFound = true;
                                    break;
                                }
                            }

                            if (mismatchFound)
                                return firstSequence;
                            else
                            {
                                if (indexOfFirstMatch > 0)
                                    return firstSequence.Take(indexOfFirstMatch).Skip(secondSequence.Count());
                                else
                                    return firstSequence.Skip(secondSequence.Count());
                            }
                        }
                    }
                }
            }
            return firstSequence;
        }

        /// <summary>
        /// Determines whether two sequences contain the same values, regardless of order.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="firstSequence">The source collection.</param>
        /// <param name="secondSequence">The collection to compare to.</param>
        /// <returns>
        /// Returns true if both sequences contain the same values.
        /// Returns false if <paramref name="secondSequence"/> is null, or the sequences do not contain the same values.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Throws when <paramref name="firstSequence"/> is null.
        /// </exception>
        internal static bool UnorderedSequenceEquals<T>(this IEnumerable<T> firstSequence, IEnumerable<T> secondSequence)
        {
            if (firstSequence == null)
                throw new ArgumentNullException("sourceCollection");
            else if (secondSequence == null)
                return false;
            else if (firstSequence.Count() != secondSequence.Count())
                return false;
            else
                foreach (T t in firstSequence)
                    if (!secondSequence.Contains(t))
                        return false;
            return true;
        }

        // TODO: Implement
        ///// <summary>
        ///// Returns a pairwise combination listing from the specified collection.
        ///// </summary>
        ///// <typeparam name="T">The type of object in the collection.</typeparam>
        ///// <param name="collection">The collection.</param>
        ///// <returns></returns>
        //internal static IEnumerable<IEnumerable<T>> PairwiseCombinations<T>(this IEnumerable<T> collection)
        //{
        //    var first = collection.First();
        //    yield return new[] { first };
        //    if (collection.CountGreaterThan(1))
        //    {
        //        var workingSet = collection.Skip(1);
        //        foreach (var remaining in workingSet)
        //        {
        //            yield return new[] { first, remaining };
        //        }
        //        if (workingSet.CountGreaterThan(1))
        //        {
        //            yield return new[] { first }.Concat(workingSet);
        //        }
        //        foreach (var pair in PairwiseCombinations(workingSet))
        //        {
        //            yield return pair;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Returns a special pairwise combination listing from the specified collection of collections which aids in accounting for various hierarchy combinations and ordering.
        ///// </summary>
        ///// <typeparam name="T">The type of object in the collection.</typeparam>
        ///// <param name="collection">The collection of collections.</param>
        ///// <returns></returns>
        //internal static IEnumerable<IEnumerable<T>> PairwiseSpecial<T>(this IEnumerable<IEnumerable<T>> collection)
        //{
        //    int totalElements = collection.Count();
        //    for (int i = 0; i < totalElements; i++)
        //    {
        //        var currentElement = collection.ElementAt(i);
        //        yield return currentElement;
        //        foreach (var collectionAgain in collection.Skip(i + 1))
        //        {
        //            if (!currentElement.First().Equals(collectionAgain.First()))
        //            {
        //                yield return currentElement.Concat(collectionAgain);
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Builds a collection combining each element in <paramref name="collection"/> with each collection in <paramref name="collections"/>.
        ///// </summary>
        ///// <typeparam name="T">The type of object in the collection.</typeparam>
        ///// <param name="collection">The collection to combine other collections to.</param>
        ///// <param name="collections">The collections to be combined.</param>
        ///// <returns></returns>
        //internal static IEnumerable<IEnumerable<T>> Build<T>(this IEnumerable<T> collection, IEnumerable<IEnumerable<T>> collections)
        //{
        //    foreach (var item in collection)
        //    {
        //        yield return new[] { item };
        //    }
        //    foreach (var item in collection)
        //    {
        //        foreach (var coll in collections)
        //        {
        //            yield return new[] { item }.Concat(coll);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Determines if the collection contains more items then the specified amount.
        ///// </summary>
        ///// <typeparam name="TSource">The type of object in the collection.</typeparam>
        ///// <param name="source">The collection.</param>
        ///// <param name="num">The number to check if the collection is greater than.</param>
        ///// <returns></returns>
        //internal static bool CountGreaterThan<TSource>(this IEnumerable<TSource> source, int num)
        //{
        //    ICollection<TSource> collectionOfT = source as ICollection<TSource>;
        //    if (collectionOfT != null) return collectionOfT.Count > num;

        //    ICollection collection = source as ICollection;
        //    if (collection != null) return collection.Count > num;

        //    int count = 0;
        //    using (IEnumerator<TSource> e = source.GetEnumerator())
        //    {
        //        checked
        //        {
        //            while (e.MoveNext()) count++;
        //            if (count > num) return true;
        //        }
        //    }
        //    return false;
        //}

    }
}
