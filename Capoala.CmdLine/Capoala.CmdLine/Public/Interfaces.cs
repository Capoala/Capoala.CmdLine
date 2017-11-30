using System.Collections.Generic;

namespace Capoala.CmdLine
{
    /// <summary>
    /// Represents the characteristics of a command line argument.
    /// </summary>
    /// <remarks>
    /// The <see cref="ICommandLineSpecification"/> interface defines the characteristics of a command line argument, such as its delimiter and hierarchal order.
    /// </remarks>
    public interface ICommandLineSpecification
    {
        /// <summary>
        /// Represents the hierarchal position in a parent-child relationship.
        /// Lower numbers represent a parent to a higher numbered child.
        /// </summary>
        int Hierarchy { get; }

        /// <summary>
        /// Represent the delimiter used for a command (switch).
        /// </summary>
        string Delimiter { get; }
    }

    /// <summary>
    /// Represents a command line argument.
    /// </summary>
    public interface ICommandLineArgument
    {
        /// <summary>
        /// The <see cref="ICommandLineSpecification"/> used to define this argument's characteristics.
        /// </summary>
        ICommandLineSpecification Specification { get; }

        /// <summary>
        /// The command - switch - value used at the command line.
        /// <para>
        /// This property will always return the associated <see cref="ICommandLineSpecification.Delimiter"/>.
        /// </para>
        /// </summary>
        string Command { get; }

        /// <summary>
        /// A description of what this argument represents or does within the scope of the application.
        /// </summary>
        string Description { get; set; }
    }

    /// <summary>
    /// Represents a paired grouping between a parent chain and its associated child arguments.
    /// <para>
    /// *See remarks for further details and correct implementation.
    /// </para>
    /// </summary>
    /// <remarks>
    /// The <see cref="ParentCallChain"/> property represents the full parent hierarchy chain. 
    /// 
    /// The <see cref="Children"/> property represents associated child pairings. 
    /// 
    /// It is important to note this property is not an "all inclusive" listing", but rather an individual grouping. 
    /// </remarks>
    /// <example>
    /// If you have one "main argument", which has three "child arguments", then specifying a grouping
    /// with "main" and "childOne" would transpose to the command line as "$ main childOne". 
    /// 
    /// If you wanted to specify a grouping that states "main" must run with "childOne" and "childTwo", but leave "childThree" as optional, 
    /// then you would create two groupings; one with "childOne" and "childTwo"; the other, with "childOne", "childTwo", and "childThree".
    /// </example>
    public interface ICommandLineGrouping
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
    /// <typeparam name="TViolationResult">The object type to return should a violation occur.</typeparam>
    public interface ICommandLineRestriction<TViolationResult>
    {
        /// <summary>
        /// Determines if a violation has occurred.
        /// </summary>
        bool IsViolated { get; }

        /// <summary>
        /// Performs all necessary queries to determine whether any violations were found, 
        /// returning all violation objects as <typeparamref name="TViolationResult"/>.
        /// </summary>
        /// <returns>
        /// Returns a collection of <typeparamref name="TViolationResult"/> representing the violations, if any violations where found.
        /// Returns an empty collection if no violations were found.
        /// </returns>
        IEnumerable<TViolationResult> GetViolations();
    }
}
