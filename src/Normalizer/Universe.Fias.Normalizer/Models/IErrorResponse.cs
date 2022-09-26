namespace Universe.Fias.Normalizer.Models
{
    /// <summary>
    /// Error response.
    /// </summary>
    public interface IErrorResponse
    {
        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        ErrorResponse Error { get; set; }
    }
}