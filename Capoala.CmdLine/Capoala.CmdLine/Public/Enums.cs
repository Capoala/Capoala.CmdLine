using System;

namespace Capoala.CmdLine
{
    /// <summary>
    /// Determines how a match is found when determining if an argument or call-chain is found.
    /// </summary>
    [Flags]
    public enum CmdLineSearchOptions
    {
        /// <summary>
        /// True if the argument or call-chain is present.
        /// </summary>
        None = 0,
        /// <summary>
        /// True if the argument or call-chain is present and contains at least one child.
        /// </summary>
        WithChildren = 1,
        /// <summary>
        /// True if the argument or call-chain is present and does not contain any children.
        /// </summary>
        WithoutChildren = 2,
        /// <summary>
        /// True if the argument or call-chain is present and contains siblings.
        /// </summary>
        WithSiblings = 4,
        /// <summary>
        /// True if the argument or call-chain is present and does not contain any siblings.
        /// </summary>
        WithoutSiblings = 8,
        /// <summary>
        /// True if the argument or call-chain is present and contains parameters.
        /// </summary>
        WithParams = 16,
        /// <summary>
        /// True if the argument or call-chain is present and does not contain paramters.
        /// </summary>
        WithoutParams = 32,
    }
}
