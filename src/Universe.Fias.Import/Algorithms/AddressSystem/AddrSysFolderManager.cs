using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Universe.Fias.DataAccess.Models;
using Universe.Helpers.Extensions;

namespace Universe.Fias.Import.Algorithms.AddressSystem
{
    /// <summary>
    /// Address system folder manager.
    /// </summary>
    public class AddrSysFolderManager
    {
        private readonly string _baseFolderPath;

        private DirectoryInfo _baseFolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddrSysFolderManager"/> class.
        /// </summary>
        /// <param name="baseFolderPath">The base folder path.</param>
        /// <exception cref="System.ArgumentNullException">baseFolderPath</exception>
        public AddrSysFolderManager([NotNull] string baseFolderPath)
        {
            if (baseFolderPath.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(baseFolderPath));

            _baseFolderPath = baseFolderPath;

            Initialize();
        }

        /// <summary>
        /// Gets the file name from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">url</exception>
        public string GetFileNameFromUrl([NotNull] string url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            var uri = new Uri(url);

            var fileName = uri.Segments.Last();
            return fileName;
        }

        /// <summary>
        /// Gets the file path by version.
        /// </summary>
        /// <param name="versionForDownload">The version for download.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// versionForDownload
        /// or
        /// fileName
        /// </exception>
        public string GetFilePathByVersion([NotNull] AsVersion versionForDownload, [NotNull] string fileName)
        {
            if (versionForDownload == null)
                throw new ArgumentNullException(nameof(versionForDownload));
            if (fileName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fileName));

            var versionFolderPath = GetOrCreateFolderForVersion(versionForDownload);
            return Path.Combine(versionFolderPath, fileName);
        }

        /// <summary>
        /// Gets the or create content archive folder for version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">version</exception>
        public string GetOrCreateContentArchiveFolderForVersion([NotNull] AsVersion version)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));

            var versionFolderPath = GetOrCreateFolderForVersion(version);

            var contentArchiveFolderPath = Path.Combine(versionFolderPath, "fias_dbf");

            if (!Directory.Exists(contentArchiveFolderPath))
                CreateFolder(contentArchiveFolderPath);

            return contentArchiveFolderPath;
        }

        /// <summary>
        /// Gets the or create folder for version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">version</exception>
        public string GetOrCreateFolderForVersion([NotNull] AsVersion version)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));

            var versionFolderName = $"ver.{version.VersionId}";
            var versionFolderPath = Path.Combine(_baseFolder.FullName, versionFolderName);
            if (!Directory.Exists(versionFolderPath))
                Directory.CreateDirectory(versionFolderPath);

            return versionFolderPath;
        }

        /// <summary>
        /// Replaces the invalid name chars.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public string ReplaceInvalidNameChars([NotNull] string name)
        {
            if (name.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(name));

            var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex($"[{Regex.Escape(regexSearch)}]");
            name = r.Replace(name, "_");

            return name;
        }

        /// <summary>
        /// Creates the folder.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        private void CreateFolder(string folderPath)
        {
            var parentFolder = Directory.GetParent(folderPath);
            if (!parentFolder.Exists)
                CreateFolder(parentFolder.FullName);

            Directory.CreateDirectory(folderPath);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            if (!Directory.Exists(_baseFolderPath))
                CreateFolder(_baseFolderPath);

            _baseFolder = new DirectoryInfo(_baseFolderPath);
        }

        /// <summary>
        /// Gets the folders by date version.
        /// </summary>
        /// <returns></returns>
        public List<DirectoryInfo> GetFoldersByDateVersion()
        {
            var regex = new Regex(@"\d{8,8}");
            return _baseFolder.GetDirectories().Where(_ => regex.IsMatch(_.Name))
                .OrderBy(_ => _.Name).ToList();
        }
    }
}