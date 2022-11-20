using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Universe.CQRS.Models.Enums;
using Universe.Diagnostic.Logger;
using Universe.Fias.Core.Infrastructure;
using Universe.Fias.DataAccess.Models;
using Universe.Fias.DataContracts;
using Universe.Fias.DataContracts.Stat;
using Universe.Fias.XML.GAR.Converter.x64.Algorithms.AddressSystem.Conversion;
using Universe.Fias.XML.GAR.Converter.x64.Algorithms.AddressSystem.Loader;
using Universe.Helpers.Extensions;
using Universe.Types;

namespace Universe.Fias.XML.GAR.Converter.x64.Algorithms.AddressSystem
{
    /// <summary>
    ///     Преобразователь данных адресной системы.
    ///     Address system data converter.
    /// </summary>
    public class AddressSystemGarConverter : DisposableObject
    {
        private EventLogger _logger;

        private readonly AppSettings _settings;
        private readonly DbSystemManagementTypes _dbType;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AddressSystemGarConverter"/> class.
        /// </summary>
        /// <param name="logger">The log</param>
        /// <param name="settings">Application settings.</param>
        /// <param name="baseFolderPath">The base folder path.</param>
        /// <param name="dbType">The database type</param>
        /// <param name="downloaderSettings">The downloader settings.</param>
        /// <exception cref="System.ArgumentNullException">baseFolderPath</exception>
        public AddressSystemGarConverter(
            EventLogger logger,
            AppSettings settings,
            [NotNull] string baseFolderPath,
            DbSystemManagementTypes dbType,
            AddrSysDownloaderSettings downloaderSettings)
        {
            if (baseFolderPath.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(baseFolderPath));

            BaseFolderPath = baseFolderPath;
            DownloaderSettings = downloaderSettings;

            FolderManager = new AddrSysFolderManager(BaseFolderPath);

            _logger = logger;
            _settings = settings;
            _dbType = dbType;
        }

        /// <summary>
        ///     Gets the base folder path.
        /// </summary>
        /// <value>
        ///     The base folder path.
        /// </value>
        public string BaseFolderPath { get; }

        /// <summary>
        ///     Gets the downloader settings.
        /// </summary>
        /// <value>
        ///     The downloader settings.
        /// </value>
        public AddrSysDownloaderSettings DownloaderSettings { get; }

        /// <summary>
        ///     Gets the folder manager.
        /// </summary>
        /// <value>
        ///     The folder manager.
        /// </value>
        public AddrSysFolderManager FolderManager { get; }

        /// <summary>
        ///     Запуск шагов.
        ///     Runs steps.
        /// </summary>
        /// <returns></returns>
        public AddrSysSyncStat Run()
        {
            var container = UnityConfig.Container;

            var resolver = new AppPrincipalResolver();
            var scope = new UniverseFiasScope(resolver, _settings, container, _dbType);

            var isProcesed = true;
            scope.UniverseFiasDb(
                db => {
                    isProcesed =
                        db.AsVersions.Count(_ => _.IsProcesed != true && (_.IsDownloadedFiasFullXml == true || _.IsDownloadedFiasDeltaXml == true)) ==
                        0;
                });

            Step01Download(scope);
            Step02Conversion(scope);

            return new AddrSysSyncStat();
        }

        /// <summary>
        ///     Шаг загрузки.
        ///     Step download.
        /// </summary>
        /// <param name="scope"></param>
        private void Step01Download(UniverseFiasScope scope)
        {
            using (var addrSysDownloader = new AddrSysDownloader(
                       scope,
                       FolderManager,
                       DownloaderSettings.FiasDownloadServiceUrl,
                       _logger))
            {
                var stepName = "Download fias dbf archive from fias service";

                if (DownloaderSettings == null || !DownloaderSettings.Enable)
                {
                    _logger.Info($"Skip step: {stepName}");
                    AddVersionIfXmlGarArchiveDbExists(scope, addrSysDownloader);
                    return;
                }

                _logger.Info(stepName);

                addrSysDownloader.CheckVersion();
                addrSysDownloader.Download();

                if (DownloaderSettings == null || !DownloaderSettings.Enable)
                {
                    AddVersionIfXmlGarArchiveDbExists(scope, addrSysDownloader);
                }
            }
        }

        private void AddVersionIfXmlGarArchiveDbExists(UniverseFiasScope scope, AddrSysDownloader addrSysDownloader)
        {
            var folders = FolderManager.GetFoldersByDateVersion();

            scope.UniverseFiasDb(
                db =>
                {
                    foreach (var directoryInfo in folders)
                    {
                        var textVerId = Regex.Match(directoryInfo.Name, @"(\d{8})").Value;
                        if (textVerId.IsNullOrEmpty())
                            continue;

                        var verId = int.Parse(textVerId);
                        var version = db.AsVersions.FirstOrDefault(_ => _.VersionId == verId && !_.IsProcesed.Value);
                        if (version != null)
                            continue;

                        var files = directoryInfo.GetFiles("*.zip", SearchOption.TopDirectoryOnly)
                            .Where(
                                _ => string.Compare(_.Name, "gar_delta_xml.zip",
                                         StringComparison.OrdinalIgnoreCase) == 0
                                     || string.Compare(_.Name, "gar_xml.zip",
                                         StringComparison.OrdinalIgnoreCase) == 0).ToList();

                        if (files.Count == 0)
                        {
                            _logger.Warning(
                                $"{directoryInfo.FullName} skiped. Not found gar_xml.zip or gar_delta_xml.zip",
                                null);
                            continue;
                        }

                        var file =
                            files.FirstOrDefault(_ =>
                                string.Compare(_.Name, "gar_xml.zip", StringComparison.OrdinalIgnoreCase) == 0);
                        if (file == null)
                            file = files.First();

                        var csvFolderPath = Path.Combine(directoryInfo.FullName, "FIAS_CSV");
                        var xmlFolderPath = Path.Combine(directoryInfo.FullName, "FIAS_GAR_XML");

                        var currDateTime = DateTime.Now;

                        var ver = addrSysDownloader.GetLastVersionFromWeb();

                        version = new AsVersion
                        {
                            VersionId = verId,
                            TextVersion = $"БД ФИАС от {currDateTime:dd.MM.yyyy}",
                            IsVersionFromFiasService = false,
                            FiasFullDbfUrl = ver.GarXMLFullURL,
                            FiasDeltaDbfUrl = ver.GarXMLDeltaURL,
                            IsDownloadedFiasFullXml =
                                string.Compare(file.Name, "gar_xml.zip", StringComparison.OrdinalIgnoreCase) == 0,
                            IsDownloadedFiasDeltaXml =
                                string.Compare(file.Name, "gar_xml.zip", StringComparison.OrdinalIgnoreCase) != 0,
                            FiasXmlArchivePath = file.FullName,
                            FiasXmlFolderPath = xmlFolderPath,
                            FiasCsvFolderPath = csvFolderPath,
                            IsProcesed = false,
                            Created = DateTimeOffset.Now
                        };

                        if (!Directory.Exists(csvFolderPath))
                            Directory.CreateDirectory(csvFolderPath);

                        if (!Directory.Exists(csvFolderPath))
                            Directory.CreateDirectory(csvFolderPath);

                        db.AsVersions.Add(version);
                        db.SaveChanges();

                        _logger.Info($"{directoryInfo.FullName} added as new version.");
                    }
                });
        }

        /// <summary>
        ///     Шаг преобразования.
        ///     Step the conversion.
        /// </summary>
        private void Step02Conversion(UniverseFiasScope scope)
        {
            var stepName = "Processing of new versions";
            _logger.Info(stepName);

            using (var addrSysConverter = new AddrSysConversionManager(
                scope,
                FolderManager))
                addrSysConverter.Transform(_logger);
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}