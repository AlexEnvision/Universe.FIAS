using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Universe.Diagnostic.Logger;
using Universe.Fias.Core.Infrastructure;
using Universe.Fias.DataAccess.Models;
using Universe.Fias.DataContracts.Stat;
using Universe.Fias.Import.Algorithms.AddressSystem.Import;
using Universe.Helpers.Extensions;
using Universe.Types;

namespace Universe.Fias.Import.Algorithms.AddressSystem
{
    /// <summary>
    ///     Sync address system data.
    /// </summary>
    public class AddressSystemSync : DisposableObject
    {
        private EventLogger _logger;

        private readonly AppSettings _settings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AddressSystemSync"/> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="settings"></param>
        /// <param name="baseFolderPath">The base folder path.</param>
        /// <exception cref="System.ArgumentNullException">baseFolderPath</exception>
        public AddressSystemSync(
            EventLogger logger,
            AppSettings settings,
            [NotNull] string baseFolderPath)
        {
            if (baseFolderPath.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(baseFolderPath));

            BaseFolderPath = baseFolderPath;
            FolderManager = new AddrSysFolderManager(BaseFolderPath);

            _logger = logger;
            _settings = settings;
        }

        /// <summary>
        /// Gets the base folder path.
        /// </summary>
        /// <value>
        /// The base folder path.
        /// </value>
        public string BaseFolderPath { get; }

        /// <summary>
        /// Gets the folder manager.
        /// </summary>
        /// <value>
        /// The folder manager.
        /// </value>
        public AddrSysFolderManager FolderManager { get; }

        /// <summary>
        /// Runs steps.
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
                Step01CheckVersion(scope);
                Step02Import(scope);
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
        ///     Шаг проверки наличия версии в БД и её создания версии, если она не обнаружена.
        ///     Step cheking version.
        /// </summary>
        /// <param name="scope"></param>
        private void Step01CheckVersion(UniverseFiasScope scope)
        {
            var stepName = "Check version and create record if isn't exist";
            _logger.Info(stepName);

            AddVersionIfDbfArchiveDbExists(scope);
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
        ///     Шаг импорта.
        ///     Step the import.
        /// </summary>
        private void Step02Import(UniverseFiasScope scope)
        {
            var stepName = "Processing of new versions";
            _logger.Info(stepName);

            using (var addrSysImporter = new AddrSysImportManager(
                scope,
                FolderManager))
                addrSysImporter.Import(_logger);
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}