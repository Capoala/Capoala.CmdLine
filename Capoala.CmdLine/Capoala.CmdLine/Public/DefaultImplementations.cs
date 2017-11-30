namespace Capoala.CmdLine
{
    /// <summary>
    /// <![CDATA[default]]> implementation of <see cref="ICommandLineSpecification"/>. This class cannot be inherited.
    /// </summary>
    public sealed class CommandLineSpecification : CommandLineSpecificationBase
    {
        /// <summary>
        /// Creates new instance of <see cref="CommandLineSpecification"/>.
        /// </summary>
        /// <param name="hierarchy">The hierarchal position in a parent-child relationship.</param>
        /// <param name="delimiter">The delimiter used for a command (switch).</param>
        /// <exception cref="System.ArgumentException">
        /// Throws when <paramref name="delimiter"/> is null or empty or contains an alpha-numeric character.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Throws when <paramref name="hierarchy"/> or <paramref name="delimiter"/> has already been declared.
        /// </exception>
        public CommandLineSpecification(int hierarchy, string delimiter) : base(hierarchy, delimiter) { }
    }

    /// <summary>
    /// A default implementation of <see cref="ICommandLineArgument"/>. This class cannot be inherited.
    /// </summary>
    public sealed class CommandLineArgument : CommandLineArgumentBase
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
        /// Throws when an object pairing with the same values as <paramref name="command"/> and <paramref name="specification"/> have already been declared.
        /// </exception>
        public CommandLineArgument(string command, ICommandLineSpecification specification, string description = null) : base(command, specification, description) { }
    }

    /// <summary>
    /// A default implementation of <see cref="ICommandLineGrouping"/>. This class cannot be inherited.
    /// </summary>
    public sealed class CommandLineGrouping : CommandLineGroupingBase
    {
        /// <summary>
        /// Creates new instance of <see cref="CommandLineGrouping"/>.
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
        public CommandLineGrouping(ICommandLineArgument parentCallChain, ICommandLineArgument[] children, string description = null) : base(parentCallChain, children, description) { }

        /// <summary>
        /// Creates new instance of <see cref="CommandLineGrouping"/>.
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
        public CommandLineGrouping(ICommandLineArgument[] parentCallChain, ICommandLineArgument[] children, string description = null) : base(parentCallChain, children, description) { }
    }
}
