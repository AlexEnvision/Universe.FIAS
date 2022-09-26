using Universe.Fias.Core.Infrastructure;

namespace Universe.Fias.DBF.Converter.x64.Algorithms.AddressSystem.Conversion
{
    /// <summary>
    ///     Address system conversion context.
    /// </summary>
    public class ConversionContext
    {
        /// <summary>
        /// Gets or sets the fias DBF archive path.
        /// </summary>
        /// <value>
        /// The fias DBF archive path.
        /// </value>
        public string FiasDbfArchivePath { get; set; }

        /// <summary>
        /// Gets or sets the fias DBF folder path.
        /// </summary>
        /// <value>
        /// The fias DBF folder path.
        /// </value>
        public string FiasDbfFolderPath { get; set; }

        /// <summary>
        /// Gets or sets the fias CSV folder path.
        /// </summary>
        /// <value>
        /// The fias CSV folder path.
        /// </value>
        public string FiasCsvFolderPath { get; set; }

        /// <summary>
        ///     Gets or sets <see cref="UniverseFiasScope"/>.
        /// </summary>
        /// <value>
        ///     The CTX.
        /// </value>
        public UniverseFiasScope Scope { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fias full DBF.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is fias full DBF; otherwise, <c>false</c>.
        /// </value>
        public bool IsFiasFullDbf { get; set; }

        /// <summary>
        /// Gets or sets the version identifier.
        /// </summary>
        /// <value>
        /// The version identifier.
        /// </value>
        public long VersionId { get; set; }
    }
}