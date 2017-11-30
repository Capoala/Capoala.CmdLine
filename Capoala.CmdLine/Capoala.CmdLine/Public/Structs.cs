namespace Capoala.CmdLine
{
    /// <summary>
    /// Represents a command line violation.
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
    }
}
