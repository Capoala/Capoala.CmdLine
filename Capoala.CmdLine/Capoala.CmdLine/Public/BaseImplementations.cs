using System;
using System.Collections.Generic;
using System.Linq;

namespace Capoala.CmdLine
{
    /// <summary>
    /// The base implementation of <see cref="ICommandLineSpecification"/>. This class can only be inherited.
    /// </summary>
    public abstract class CommandLineSpecificationBase : ICommandLineSpecification, IEquatable<CommandLineSpecificationBase>
    {
        /// <summary>
        /// Represents the hierarchal position in a parent-child relationship.
        /// Lower numbers represent a parent to a higher numbered child.
        /// </summary>
        public int Hierarchy { get; }

        /// <summary>
        /// Represent the delimiter used for a command (switch).
        /// </summary>
        public string Delimiter { get; }

        /// <summary>
        /// Creates new instance of <see cref="CommandLineSpecificationBase"/>.
        /// </summary>
        /// <param name="hierarchy">The hierarchal position in a parent-child relationship.</param>
        /// <param name="delimiter">The delimiter used for a command (switch).</param>
        /// <exception cref="System.ArgumentException">
        /// Throws when <paramref name="delimiter"/> is null or empty or contains an alpha-numeric character.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Throws when <paramref name="hierarchy"/> or <paramref name="delimiter"/> has already been declared.
        /// </exception>
        protected CommandLineSpecificationBase(int hierarchy, string delimiter)
        {
            if (string.IsNullOrWhiteSpace(delimiter) || delimiter.ToCharArray().Any(c => char.IsLetterOrDigit(c)))
                throw new ArgumentException("The value is not supported.", nameof(delimiter));

            if (CommandLineSetManager.KnownSpecifications.Any(spec => spec.Hierarchy == hierarchy))
                throw new ArgumentException("An object with the same value has already been declared.", nameof(hierarchy));

            if (CommandLineSetManager.KnownSpecifications.Any(spec => spec.Delimiter == delimiter))
                throw new ArgumentException("An object with the same value has already been declared.", nameof(delimiter));

            Hierarchy = hierarchy;
            Delimiter = delimiter;

            if (!CommandLineSetManager.KnownSpecifications.Add(this))
                throw new ArgumentException("An object with the same value has already been declared.");
        }

        #region IEquatable
        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// Returns true if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        public override bool Equals(object obj) => Equals(obj as CommandLineSpecificationBase);

        /// <summary>
        /// Determines whether this instance and another specified <see cref="CommandLineSpecificationBase"/> object have the same value.
        /// </summary>
        /// <param name="other">The <see cref="CommandLineSpecificationBase"/> to compare to this instance.</param>
        /// <returns>
        /// Returns true if the value of the value parameter is the same as the value of this instance; otherwise, false. 
        /// If value is null, the method returns false.
        /// </returns>
        public bool Equals(CommandLineSpecificationBase other) => other != null && Hierarchy == other.Hierarchy && Delimiter == other.Delimiter;

        /// <summary>
        /// Returns the hash code for this <see cref="CommandLineSpecificationBase"/>.
        /// </summary>
        /// <returns>
        /// Returns a 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = 480111778;
            hashCode = hashCode * -1521134295 + Hierarchy.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Delimiter);
            return hashCode;
        }

        /// <summary>
        /// Determines whether two specified <see cref="CommandLineSpecificationBase"/> objects have the same value.
        /// </summary>
        /// <param name="base1">The first <see cref="CommandLineSpecificationBase"/> to compare, or null.</param>
        /// <param name="base2">The second <see cref="CommandLineSpecificationBase"/> to compare, or null.</param>
        /// <returns>
        /// Returns true if the value of <paramref name="base1"/> is the same as the value of <paramref name="base2"/>; otherwise, false.
        /// </returns>
        public static bool operator ==(CommandLineSpecificationBase base1, CommandLineSpecificationBase base2) => EqualityComparer<CommandLineSpecificationBase>.Default.Equals(base1, base2);

        /// <summary>
        /// Determines whether two specified <see cref="CommandLineSpecificationBase"/> objects have different values.
        /// </summary>
        /// <param name="base1">The first <see cref="CommandLineSpecificationBase"/> to compare, or null.</param>
        /// <param name="base2">The second <see cref="CommandLineSpecificationBase"/> to compare, or null.</param>
        /// <returns>
        /// Returns true if the value of <paramref name="base1"/> is different from the value of <paramref name="base2"/>; otherwise, false.
        /// </returns>
        public static bool operator !=(CommandLineSpecificationBase base1, CommandLineSpecificationBase base2) => !(base1 == base2);
        #endregion
    }

    /// <summary>
    /// The base implementation of <see cref="ICommandLineArgument"/>. This class can only be inherited.
    /// </summary>
    public abstract class CommandLineArgumentBase : ICommandLineArgument, IEquatable<CommandLineArgumentBase>
    {
        /// <summary>
        /// TThe <see cref="ICommandLineSpecification"/> used to define this argument's characteristics.
        /// </summary>
        public ICommandLineSpecification Specification { get; }

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
        /// Throws when an object pairing with the same values as <paramref name="command"/> and <paramref name="specification"/> have already been declared.
        /// </exception>
        protected CommandLineArgumentBase(string command, ICommandLineSpecification specification, string description = null)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));

            Specification = specification ?? throw new ArgumentNullException(nameof(specification));

            if (CommandLineSetManager.KnownArguments.Any(arg => arg.Command.Equals(command, CommandLine.Comparer) && arg.Specification == specification))
                throw new ArgumentException("An object with the same value has already been declared.");

            Command = $"{specification.Delimiter}{command.TrimStart(specification.Delimiter.ToCharArray())}";
            Description = description;

            if (!CommandLineSetManager.KnownArguments.Add(this))
                throw new ArgumentException("An object with the same value has already been declared.");
        }

        #region IEquatable
        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// Returns true if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        public override bool Equals(object obj) => Equals(obj as CommandLineArgumentBase);

        /// <summary>
        /// Determines whether this instance and another specified <see cref="CommandLineArgumentBase"/> object have the same value.
        /// </summary>
        /// <param name="other">The <see cref="CommandLineArgumentBase"/> to compare to this instance.</param>
        /// <returns>
        /// Returns true if the value of the value parameter is the same as the value of this instance; otherwise, false. 
        /// If value is null, the method returns false.
        /// </returns>
        public bool Equals(CommandLineArgumentBase other) => other != null && EqualityComparer<ICommandLineSpecification>.Default.Equals(Specification, other.Specification) && Command == other.Command && Description == other.Description;

        /// <summary>
        /// Returns the hash code for this <see cref="CommandLineArgumentBase"/>.
        /// </summary>
        /// <returns>
        /// Returns a 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = -1464717235;
            hashCode = hashCode * -1521134295 + EqualityComparer<ICommandLineSpecification>.Default.GetHashCode(Specification);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Command);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
            return hashCode;
        }

        /// <summary>
        /// Determines whether two specified <see cref="CommandLineArgumentBase"/> objects have the same value.
        /// </summary>
        /// <param name="base1">The first <see cref="CommandLineArgumentBase"/> to compare, or null.</param>
        /// <param name="base2">The second <see cref="CommandLineArgumentBase"/> to compare, or null.</param>
        /// <returns>
        /// Returns true if the value of <paramref name="base1"/> is the same as the value of <paramref name="base2"/>; otherwise, false.
        /// </returns>
        public static bool operator ==(CommandLineArgumentBase base1, CommandLineArgumentBase base2) => EqualityComparer<CommandLineArgumentBase>.Default.Equals(base1, base2);

        /// <summary>
        /// Determines whether two specified <see cref="CommandLineArgumentBase"/> objects have different values.
        /// </summary>
        /// <param name="base1">The first <see cref="CommandLineArgumentBase"/> to compare, or null.</param>
        /// <param name="base2">The second <see cref="CommandLineArgumentBase"/> to compare, or null.</param>
        /// <returns>
        /// Returns true if the value of <paramref name="base1"/> is different from the value of <paramref name="base2"/>; otherwise, false.
        /// </returns>
        public static bool operator !=(CommandLineArgumentBase base1, CommandLineArgumentBase base2) => !(base1 == base2);
        #endregion
    }

    /// <summary>
    /// The base implementation of <see cref="ICommandLineGrouping"/>. This class can only be inherited.
    /// </summary>
    public abstract class CommandLineGroupingBase : ICommandLineGrouping, IEquatable<CommandLineGroupingBase>
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
                throw new ArgumentNullException(nameof(parentCallChain));

            ParentCallChain = parentCallChain;
            Children = children ?? new ICommandLineArgument[] { };
            Description = description;
        }

        #region IEquatable
        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// Returns true if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        public override bool Equals(object obj) => Equals(obj as CommandLineGroupingBase);

        /// <summary>
        /// Determines whether this instance and another specified <see cref="CommandLineGroupingBase"/> object have the same value.
        /// </summary>
        /// <param name="other">The <see cref="CommandLineGroupingBase"/> to compare to this instance.</param>
        /// <returns>
        /// Returns true if the value of the value parameter is the same as the value of this instance; otherwise, false. 
        /// If value is null, the method returns false.
        /// </returns>
        public bool Equals(CommandLineGroupingBase other) => other != null && EqualityComparer<ICommandLineArgument[]>.Default.Equals(ParentCallChain, other.ParentCallChain) && EqualityComparer<ICommandLineArgument[]>.Default.Equals(Children, other.Children) && Description == other.Description;

        /// <summary>
        /// Returns the hash code for this <see cref="CommandLineGroupingBase"/>.
        /// </summary>
        /// <returns>
        /// Returns a 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = 775772701;
            hashCode = hashCode * -1521134295 + EqualityComparer<ICommandLineArgument[]>.Default.GetHashCode(ParentCallChain);
            hashCode = hashCode * -1521134295 + EqualityComparer<ICommandLineArgument[]>.Default.GetHashCode(Children);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
            return hashCode;
        }

        /// <summary>
        /// Determines whether two specified <see cref="CommandLineGroupingBase"/> objects have the same value.
        /// </summary>
        /// <param name="base1">The first <see cref="CommandLineGroupingBase"/> to compare, or null.</param>
        /// <param name="base2">The second <see cref="CommandLineGroupingBase"/> to compare, or null.</param>
        /// <returns>
        /// Returns true if the value of <paramref name="base1"/> is the same as the value of <paramref name="base2"/>; otherwise, false.
        /// </returns>
        public static bool operator ==(CommandLineGroupingBase base1, CommandLineGroupingBase base2) => EqualityComparer<CommandLineGroupingBase>.Default.Equals(base1, base2);

        /// <summary>
        /// Determines whether two specified <see cref="CommandLineGroupingBase"/> objects have different values.
        /// </summary>
        /// <param name="base1">The first <see cref="CommandLineGroupingBase"/> to compare, or null.</param>
        /// <param name="base2">The second <see cref="CommandLineGroupingBase"/> to compare, or null.</param>
        /// <returns>
        /// Returns true if the value of <paramref name="base1"/> is different from the value of <paramref name="base2"/>; otherwise, false.
        /// </returns>
        public static bool operator !=(CommandLineGroupingBase base1, CommandLineGroupingBase base2) => !(base1 == base2);
        #endregion
    }

    /// <summary>
    /// The base implementation of <see cref="ICommandLineRestriction{T}"/>. This class can only be inherited.
    /// </summary>
    /// <typeparam name="TViolationResult">The object type to return should a violation occur.</typeparam>
    public abstract class CommandLineRestrictionBase<TViolationResult> : ICommandLineRestriction<TViolationResult>, IEquatable<CommandLineRestrictionBase<TViolationResult>>
    {
        /// <summary>
        /// Determines if a violation has occurred.
        /// </summary>
        public virtual bool IsViolated => GetViolations().Any();

        /// <summary>
        /// Performs all necessary queries to determine whether any violations were found, returning all violation objects as <typeparamref name="TViolationResult"/>.
        /// </summary>
        /// <returns>
        /// Returns a collection of <typeparamref name="TViolationResult"/> representing the violations, if any violations where found.
        /// Returns an empty collection if no violations were found.
        /// </returns>
        public abstract IEnumerable<TViolationResult> GetViolations();

        #region IEquatable
        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// Returns true if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        public override bool Equals(object obj) => Equals(obj as CommandLineRestrictionBase<TViolationResult>);

        /// <summary>
        /// Determines whether this instance and another specified <see cref="CommandLineSpecificationBase"/> object have the same value.
        /// </summary>
        /// <param name="other">The <see cref="CommandLineRestrictionBase{TViolationResult}"/> to compare to this instance.</param>
        /// <returns>
        /// Returns true if the value of the value parameter is the same as the value of this instance; otherwise, false. 
        /// If value is null, the method returns false.
        /// </returns>
        public bool Equals(CommandLineRestrictionBase<TViolationResult> other) => other != null && IsViolated == other.IsViolated;

        /// <summary>
        /// Returns the hash code for this <see cref="CommandLineRestrictionBase{TViolationResult}"/>.
        /// </summary>
        /// <returns>
        /// Returns a 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode() => -10863425 + IsViolated.GetHashCode();

        /// <summary>
        /// Determines whether two specified <see cref="CommandLineRestrictionBase{TViolationResult}"/> objects have the same value.
        /// </summary>
        /// <param name="base1">The first <see cref="CommandLineRestrictionBase{TViolationResult}"/> to compare, or null.</param>
        /// <param name="base2">The second <see cref="CommandLineRestrictionBase{TViolationResult}"/> to compare, or null.</param>
        /// <returns>
        /// Returns true if the value of <paramref name="base1"/> is the same as the value of <paramref name="base2"/>; otherwise, false.
        /// </returns>
        public static bool operator ==(CommandLineRestrictionBase<TViolationResult> base1, CommandLineRestrictionBase<TViolationResult> base2) => EqualityComparer<CommandLineRestrictionBase<TViolationResult>>.Default.Equals(base1, base2);

        /// <summary>
        /// Determines whether two specified <see cref="CommandLineRestrictionBase{TViolationResult}"/> objects have different values.
        /// </summary>
        /// <param name="base1">The first <see cref="CommandLineRestrictionBase{TViolationResult}"/> to compare, or null.</param>
        /// <param name="base2">The second <see cref="CommandLineRestrictionBase{TViolationResult}"/> to compare, or null.</param>
        /// <returns>
        /// Returns true if the value of <paramref name="base1"/> is different from the value of <paramref name="base2"/>; otherwise, false.
        /// </returns>
        public static bool operator !=(CommandLineRestrictionBase<TViolationResult> base1, CommandLineRestrictionBase<TViolationResult> base2) => !(base1 == base2);
        #endregion
    }
}
