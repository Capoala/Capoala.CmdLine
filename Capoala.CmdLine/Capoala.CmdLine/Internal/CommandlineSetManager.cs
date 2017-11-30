using System.Collections.Generic;

namespace Capoala.CmdLine
{
    /// <summary>
    /// An internal class designed to store behind-the-scenes data that could othewise cause undesirable effects to the interworkings of the assembly.
    /// </summary>
    internal static class CommandLineSetManager
    {
        /// <summary>
        /// A collection of instatiated <see cref="ICommandLineSpecification"/> objects.
        /// </summary>
        internal static HashSet<ICommandLineSpecification> KnownSpecifications { get; set; } = new HashSet<ICommandLineSpecification>();

        /// <summary>
        /// A collection of instatiated <see cref="ICommandLineArgument"/> objects.
        /// </summary>
        internal static HashSet<ICommandLineArgument> KnownArguments { get; set; } = new HashSet<ICommandLineArgument>();
    }
}
