namespace Universe.Fias.Normalizer.Models
{
    /// <summary>
    /// Error response.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Gets or sets the message error.
        /// </summary>
        /// <value>
        /// The message error.
        /// </value>
        public string MessageError { get; set; }

        /// <summary>
        /// Gets or sets the message for user.
        /// </summary>
        /// <value>
        /// The message for user.
        /// </value>
        public string MessageForUser { get; set; }

        /// <summary>
        /// Gets or sets the stack trace.
        /// </summary>
        /// <value>
        /// The stack trace.
        /// </value>
        public string StackTrace { get; set; }
    }
}