using Universe.Fias.Core.Infrastructure;

namespace Universe.Fias.XML.GAR.Converter.x64.Algorithms.AddressSystem.Conversion
{
    /// <summary>
    ///     Address system conversion context.
    /// </summary>
    public class ConversionContext
    {
        /// <summary>
        /// Gets or sets the fias Xml archive path.
        /// </summary>
        /// <value>
        /// The fias Xml archive path.
        /// </value>
        public string FiasXmlArchivePath { get; set; }

        /// <summary>
        /// Gets or sets the fias Xml folder path.
        /// </summary>
        /// <value>
        /// The fias Xml folder path.
        /// </value>
        public string FiasXmlFolderPath { get; set; }

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
        /// Gets or sets a value indicating whether this instance is fias full Xml.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is fias full Xml; otherwise, <c>false</c>.
        /// </value>
        public bool IsFiasFullXml { get; set; }

        /// <summary>
        /// Gets or sets the version identifier.
        /// </summary>
        /// <value>
        /// The version identifier.
        /// </value>
        public long VersionId { get; set; }
    }
}