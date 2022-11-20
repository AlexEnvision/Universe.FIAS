using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Universe.DataAccess.Models;

namespace Universe.Fias.DataAccess.Models
{
    public class AsVersion : Entity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AsVersion()
        {
            AsVersionFiles = new HashSet<AsVersionFile>();
        }

        public int VersionId { get; set; }

        public string TextVersion { get; set; }

        public string FiasFullDbfUrl { get; set; }

        public bool? IsDownloadedFiasFullDbf { get; set; }

        public string FiasDeltaDbfUrl { get; set; }

        public bool? IsDownloadedFiasDeltaDbf { get; set; }

        public bool? IsProcesed { get; set; }

        public bool IsVersionFromFiasService { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? StartDateDownload { get; set; }

        public DateTimeOffset? EndDateDownload { get; set; }

        public string FiasDbfArchivePath { get; set; }

        public string FiasDbfFolderPath { get; set; }

        public string FiasCsvFolderPath { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AsVersionFile> AsVersionFiles { get; set; }

        /// <summary>
        ///     Путь к папке с XML GAR
        /// </summary>
        public string FiasXmlFolderPath { get; set; }

        /// <summary>
        ///     Путь к архиву с XML GAR
        /// </summary>
        public string FiasXmlArchivePath { get; set; }

        /// <summary>
        ///     Загружен полный GAR XML?
        /// </summary>
        public bool? IsDownloadedFiasFullXml { get; set; }

        /// <summary>
        ///     Загружена изменения GAR XML?
        /// </summary>
        public bool? IsDownloadedFiasDeltaXml { get; set; }
    }
}