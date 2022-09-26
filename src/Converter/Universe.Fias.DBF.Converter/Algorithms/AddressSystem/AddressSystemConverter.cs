using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Universe.Diagnostic.Logger;
using Universe.Fias.Core.Infrastructure;
using Universe.Fias.DataAccess.Models;
using Universe.Fias.DataContracts;
using Universe.Fias.DataContracts.Stat;
using Universe.Fias.DBF.Converter.x64.Algorithms.AddressSystem.Conversion;
using Universe.Helpers.Extensions;
using Universe.Types;

namespace Universe.Fias.DBF.Converter.x64.Algorithms.AddressSystem
{
    /// <summary>
    ///     Преобразователь данных адресной системы.
    ///     Address system data converter.
    /// </summary>
    public class AddressSystemConverter : DisposableObject
    {
        private EventLogger _logger;

        private readonly AppSettings _settings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AddressSystemConverter"/> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="settings"></param>
        /// <param name="baseFolderPath">The base folder path.</param>
        /// <param name="downloaderSettings">The downloader settings.</param>
        /// <exception cref="System.ArgumentNullException">baseFolderPath</exception>
        public AddressSystemConverter(
            EventLogger logger,
            AppSettings settings,
            [NotNull] string baseFolderPath,
            AddrSysDownloaderSettings downloaderSettings)
        {
            if (baseFolderPath.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(baseFolderPath));

            BaseFolderPath = baseFolderPath;
            DownloaderSettings = downloaderSettings;

            FolderManager = new AddrSysFolderManager(BaseFolderPath);

            _logger = logger;
            _settings = settings;
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
            var scope = new UniverseFiasScope(resolver, _settings, container);

            var isProcesed = true;
            scope.UniverseFiasDb(
                db => {
                    isProcesed =
                        db.AsVersions.Count(_ => _.IsProcesed != true && (_.IsDownloadedFiasFullDbf == true || _.IsDownloadedFiasDeltaDbf == true)) ==
                        0;
                });

            //if (isProcesed)
            {
                Step01Download(scope);
                Step02Conversion(scope);
            }
            //else
            //{
            //    Task.WaitAll(
            //        Task.Run(() => Step01Download()),
            //        Task.Run(() => Step02Import()));

            //    isProcesed = true;
            //    scope.UniverseFiasDb(
            //        db => {
            //            isProcesed =
            //                db.AsVersions.Count(
            //                    _ => _.IsProcesed != true && (_.IsDownloadedFiasFullDbf == true || _.IsDownloadedFiasDeltaDbf == true)) == 0;
            //        });

            //    if (!isProcesed)
            //        Step02Import();
            //}

            return new AddrSysSyncStat();
        }

        /// <summary>
        ///     Шаг загрузки.
        ///     Step download.
        /// </summary>
        /// <param name="scope"></param>
        private void Step01Download(UniverseFiasScope scope)
        {
            var stepName = "Download fias dbf archive from fias service";

            if (DownloaderSettings == null || !DownloaderSettings.Enable)
            {
                _logger.Info($"Skip step: {stepName}");
                AddVersionIfDbfArchiveDbExists(scope);
                return;
            }

            _logger.Info(stepName);

            using (var addrSysDownloader = new AddrSysDownloader(
                scope,
                FolderManager,
                DownloaderSettings.FiasDownloadServiceUrl,
                _logger))
            {
                addrSysDownloader.CheckVersion();
                addrSysDownloader.Download();
            }

            if (DownloaderSettings == null || !DownloaderSettings.Enable)
            {
                AddVersionIfDbfArchiveDbExists(scope);
            }
        }

        private void AddVersionIfDbfArchiveDbExists(UniverseFiasScope scope)
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
                                _ => string.Compare(_.Name, "fias_delta_dbf.zip",
                                         StringComparison.OrdinalIgnoreCase) == 0
                                     || string.Compare(_.Name, "fias_dbf.zip",
                                         StringComparison.OrdinalIgnoreCase) == 0).ToList();

                        if (files.Count == 0)
                        {
                            _logger.Warning(
                                $"{directoryInfo.FullName} skiped. Not found fias_dbf.zip or fias_delta_dbf.zip",
                                null);
                            continue;
                        }

                        var file =
                            files.FirstOrDefault(_ =>
                                string.Compare(_.Name, "fias_dbf.zip", StringComparison.OrdinalIgnoreCase) == 0);
                        if (file == null)
                            file = files.First();

                        var csvFolderPath = Path.Combine(directoryInfo.FullName, "FIAS_CSV");
                        var dbfFolderPath = Path.Combine(directoryInfo.FullName, "FIAS_DBF");

                        var currDateTime = DateTime.Now;

                        version = new AsVersion
                        {
                            VersionId = verId,
                            TextVersion = $"БД ФИАС от {currDateTime:dd.MM.yyyy}",
                            IsVersionFromFiasService = false,
                            FiasFullDbfUrl = "https://fias-file.nalog.ru/downloads/2021.08.31/fias_dbf.zip",
                            FiasDeltaDbfUrl = "https://fias-file.nalog.ru/downloads/2021.08.31/fias_delta_dbf.zip",
                            IsDownloadedFiasFullDbf =
                                string.Compare(file.Name, "fias_dbf.zip", StringComparison.OrdinalIgnoreCase) == 0,
                            IsDownloadedFiasDeltaDbf =
                                string.Compare(file.Name, "fias_dbf.zip", StringComparison.OrdinalIgnoreCase) != 0,
                            FiasDbfArchivePath = file.FullName,
                            FiasDbfFolderPath = dbfFolderPath,
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