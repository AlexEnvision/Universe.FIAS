namespace Universe.Fias.DataContracts
{
    /// <summary>
    /// Address system downloader settings.
    /// </summary>
    public class AddrSysDownloaderSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AddrSysDownloaderSettings"/> is enable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enable; otherwise, <c>false</c>.
        /// </value>
        public bool Enable { get; set; }

        /// <summary>
        /// Gets or sets the fias download service URL.
        /// </summary>
        /// <value>
        /// The fias download service URL.
        /// </value>
        public string FiasDownloadServiceUrl { get; set; }
    }
}