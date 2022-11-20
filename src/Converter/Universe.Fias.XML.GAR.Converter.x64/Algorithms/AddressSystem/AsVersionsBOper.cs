using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Universe.Fias.DataAccess;
using Universe.Fias.DataAccess.Models;
using Universe.Helpers.Extensions;

namespace Universe.Fias.XML.GAR.Converter.x64.Algorithms.AddressSystem
{
    /// <summary>
    /// Address system operations on versions.
    /// </summary>
    public class AsVersionsBOper
    {
        private readonly IUniverseFiasDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsVersionsBOper"/> class.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <exception cref="System.ArgumentNullException">db</exception>
        public AsVersionsBOper([NotNull] IUniverseFiasDbContext db)
        {
            if (db == null)
                throw new ArgumentNullException(nameof(db));

            _db = db;
        }

        /// <summary>
        /// Adds the version.
        /// </summary>
        /// <param name="version">The version.</param>
        public void AddVersion(AsVersion version)
        {
            version.Created = DateTimeOffset.Now;
            _db.AsVersions.Add(version);
        }

        /// <summary>
        /// Gets the last Conversioned version.
        /// </summary>
        /// <returns></returns>
        public AsVersion GetLastConversionedVersion()
        {
            var asVersion = _db.AsVersions.Where(_ => _.IsProcesed == true)
                .OrderByDescending(_ => _.VersionId).FirstOrDefault();
            return asVersion;
        }

        /// <summary>
        /// Gets the last Conversioned version Full.
        /// </summary>
        /// <returns></returns>
        public AsVersion GetLastConversionedVersionFullDbf()
        {
            var asVersion = _db.AsVersions.Where(_ => _.IsProcesed == true && _.IsDownloadedFiasFullXml == true)
                .OrderByDescending(_ => _.VersionId).FirstOrDefault();
            return asVersion;
        }

        /// <summary>
        /// Gets the last version.
        /// </summary>
        /// <returns></returns>
        public AsVersion GetLastVersion()
        {
            var asVersion = _db.AsVersions.OrderByDescending(_ => _.VersionId).FirstOrDefault();
            return asVersion;
        }

        /// <summary>
        /// Gets the versions for download.
        /// </summary>
        /// <param name="versionId">The version identifier.</param>
        /// <returns></returns>
        public List<AsVersion> GetVersionsForDownload(int? versionId)
        {
            var query = _db.AsVersions
                .Where(_ => _.IsVersionFromFiasService && !(_.IsDownloadedFiasFullXml == true || _.IsDownloadedFiasDeltaXml == true));
            if (versionId != null)
                query = query.Where(_ => _.VersionId > versionId);

            var versions = query.OrderBy(_ => _.VersionId)
                .ToList();
            return versions;
        }

        /// <summary>
        /// Gets the versions for Conversion.
        /// </summary>
        /// <returns></returns>
        public List<AsVersion> GetVersionsForConversion()
        {
            var query = _db.AsVersions
                .Where(_ => _.IsProcesed == false);

            var versions = query.OrderBy(_ => _.VersionId)
                .ToList();
            return versions;
        }

        /// <summary>
        /// Updates the version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <exception cref="System.ArgumentNullException">version</exception>
        public void UpdateVersion([NotNull] AsVersion version)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));

            _db.Entry(version).State = EntityState.Modified;

            _db.SaveChanges();
        }

        /// <summary>
        /// Updates the version end download.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="filePath">The file path.</param>
        /// <exception cref="System.ArgumentNullException">filePath</exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.NotSupportedException"></exception>
        public void UpdateVersionEndDownload(long id, [NotNull] string filePath)
        {
            if (filePath.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(filePath));

            var version = _db.AsVersions.FirstOrDefault(_ => _.Id == id);
            if (version == null)
                throw new InvalidOperationException($"AsVersion with id={id} not found");


            if (version.IsDownloadedFiasFullXml != null)
            {
                version.IsDownloadedFiasFullXml = true;
                version.IsDownloadedFiasDeltaXml = null;
            }
            else if (version.IsDownloadedFiasDeltaXml != null)
            {
                version.IsDownloadedFiasFullXml = null;
                version.IsDownloadedFiasDeltaXml = true;
            }
            else
            {
                throw new NotSupportedException($"Unknown type download (IsDownloadedFiasFullXml or IsDownloadedFiasDeltaXml)");
            }

            version.EndDateDownload = DateTimeOffset.Now;
            version.FiasXmlArchivePath = filePath;
            version.IsProcesed = false;

            UpdateVersion(version);
        }

        /// <summary>
        /// Updates the version start download.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="isFullDbf">if set to <c>true</c> [is full DBF].</param>
        public void UpdateVersionStartDownload(AsVersion version, bool isFullDbf)
        {
            if (isFullDbf)
            {
                version.IsDownloadedFiasFullXml = false;
                version.IsDownloadedFiasDeltaXml = null;
            }
            else
            {
                version.IsDownloadedFiasFullXml = null;
                version.IsDownloadedFiasDeltaXml = false;
            }

            version.StartDateDownload = DateTimeOffset.Now;

            UpdateVersion(version);
        }
    }
}