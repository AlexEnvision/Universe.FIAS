using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using DownloadServiceRef;
using Universe.Diagnostic.Logger;
using Universe.Fias.Core.Infrastructure;
using Universe.Fias.DataAccess.Models;
using Universe.Fias.Helpers;
using Universe.Helpers.Extensions;
using Universe.Types;

namespace Universe.Fias.DBF.Converter.x64.Algorithms.AddressSystem
{
    /// <summary>
    /// Address system new versions downloader.
    /// </summary>
    /// <seealso cref="DisposableObject"/>
    public class AddrSysDownloader : DisposableObject
    {
        private readonly string _fiasDownloadServiceUrl;

        private readonly AddrSysFolderManager _folderManager;

        private readonly UniverseFiasScope _scope;

        private EventLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddrSysDownloader"/> class.
        /// </summary>
        /// <param name="scope">Scope</param>
        /// <param name="folderManager">The folder manager.</param>
        /// <param name="fiasDownloadServiceUrl">The fias download service URL.</param>
        /// <param name="logger"></param>
        /// <exception cref="System.ArgumentNullException">
        /// Ctx
        /// or
        /// folderManager
        /// or
        /// fiasDownloadServiceUrl
        /// </exception>
        public AddrSysDownloader(
            [NotNull] UniverseFiasScope scope,
            [NotNull] AddrSysFolderManager folderManager,
            [NotNull] string fiasDownloadServiceUrl,
            [NotNull] EventLogger logger)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (folderManager == null)
                throw new ArgumentNullException(nameof(folderManager));
            if (fiasDownloadServiceUrl.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fiasDownloadServiceUrl));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _scope = scope;
            _fiasDownloadServiceUrl = fiasDownloadServiceUrl;
            _folderManager = folderManager;
            _logger = logger;
        }

        /// <summary>
        /// Gets or sets the fake replace URL for download.
        /// </summary>
        /// <value>
        /// The fake replace URL for download.
        /// </value>
        public static string FakeReplaceUrlForDownload { get; set; }

        /// <summary>
        /// Checks the new version.
        /// </summary>
        public void CheckVersion()
        {
            _scope.UniverseFiasDb(
                db => {
                    var oper = new AsVersionsBOper(db);
                    var lastVersion = oper.GetLastVersion();

                    var newVersions = GetNewVersions(lastVersion);

                    if (newVersions.Count <= 0)
                    {
                        _logger.Info("There are no new versions");
                        return;
                    }

                    _logger.Info($"{newVersions.Count} - new versions");

                    newVersions
                        .OrderBy(_ => _.VersionId)
                        .ToList()
                        .ForEach(
                            _ => oper.AddVersion(
                                new AsVersion {
                                    VersionId = (int)_.VersionId,
                                    FiasFullDbfUrl = _.FiasCompleteDbfUrl,
                                    FiasDeltaDbfUrl = _.FiasDeltaDbfUrl,
                                    TextVersion = _.TextVersion,
                                    IsVersionFromFiasService = true
                                }));

                    db.SaveChanges();
                });
        }

        /// <summary>
        /// Download new versions.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">_logger</exception>
        public void Download()
        {
            _scope.UniverseFiasDb(
                db => {
                    var oper = new AsVersionsBOper(db);
                    var lastProcessedVersion = oper.GetLastConversionedVersion();
                    var versionsForDownload = oper.GetVersionsForDownload(lastProcessedVersion?.VersionId);

                    if (versionsForDownload.Count <= 0)
                    {
                        _logger.Info("No download available");
                        return;
                    }

                    var lastProcessedVersionFull = oper.GetLastConversionedVersionFullDbf();
                    var tasks = new List<Task>();
                    if (lastProcessedVersion == null || lastProcessedVersionFull.VersionId < 521)
                    {
                        // Если нет ни одной обработанной версии, то загружаем полную последнеею
                        _logger.Info("If there is not a single processed version, then load the full last");
                        var versionForDownload = versionsForDownload.Last();
                        StartDownload(tasks, oper, versionForDownload, true);
                    }
                    else
                    {
                        foreach (var versionForDownload in versionsForDownload)
                        {
                            // Если есть обработанные версии, то загружаем изменения
                            _logger.Info("If there are processed versions, then we load the changes");
                            StartDownload(tasks, oper, versionForDownload, false);
                        }
                    }

                    _logger.Info("We will wait for the completion of tasks");
                    Task.WaitAll(tasks.ToArray());
                    _logger.Info("Finally we waited");
                });
        }

        /// <summary>
        ///     <see href="https://msdn.microsoft.com/library/ms244737.aspx">CA1063: следует правильно реализовывать IDisposable</see>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            // Пример для реализации в наследниках для управляемых ресурсов
            if (disposing)
            {
                // free managed resources
                //managedResource?.Dispose();
                //managedResource = null;
            }

            //     // Пример для реализации в наследниках для не управляемых ресурсов
            //     // free native resources if there are any.
            //     if (nativeResource != IntPtr.Zero)
            //     {
            //Marshal.FreeHGlobal(nativeResource);
            //nativeResource = IntPtr.Zero;
            //     }
        }

        /// <summary>
        /// Creates the fias download service client.
        /// </summary>
        /// <returns></returns>
        internal DownloadServiceClient CreateFiasDownloadServiceClient()
        {
            var svcClient = new DownloadServiceClient(
                new BasicHttpBinding {
                    MaxReceivedMessageSize = 1024 * 500,
                    Security = new BasicHttpSecurity {
                        Mode = BasicHttpSecurityMode.Transport
                    }
                },
                new EndpointAddress(_fiasDownloadServiceUrl));
            return svcClient;
        }

        /// <summary>
        /// Gets the new versions.
        /// </summary>
        /// <param name="lastVersion">The last version.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        /// </exception>
        internal List<DownloadFileInfo> GetNewVersions(AsVersion lastVersion)
        {
            var svcClient = CreateFiasDownloadServiceClient();

            var result = AsyncHelper.RunSync(() => svcClient.GetLastDownloadFileInfoAsync());
            if (result == null)
                throw new InvalidOperationException($"GetLastDownloadFileInfo return null");

            var ver = result.Body.GetLastDownloadFileInfoResult;

            if (lastVersion == null) // значит нужна самая последня версия для полной загрузки
                return new List<DownloadFileInfo>
                {
                    ver
                };

            if (lastVersion.VersionId == ver.VersionId) // версии совпадают, новых нет
                return new List<DownloadFileInfo>();

            if (lastVersion.VersionId + 1 == ver.VersionId) // есть одна новая версия (пропущенных версий нет)
                return new List<DownloadFileInfo>
                {
                    ver
                };

            var allVersions = AsyncHelper.RunSync(() => svcClient.GetAllDownloadFileInfoAsync());
            if (allVersions == null)
                throw new InvalidOperationException($"GetAllDownloadFileInfo return null");

            var newVersions = allVersions
                .Where(_ => _.VersionId > lastVersion.VersionId)
                .OrderBy(_ => _.VersionId)
                .ToList();
            return newVersions;
        }

        /// <summary>
        /// Starts the download.
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        /// <param name="oper">The oper.</param>
        /// <param name="versionForDownload">The version for download.</param>
        /// <param name="isFullDbf">if set to <c>true</c> [is full DBF].</param>
        private void StartDownload(
            List<Task> tasks,
            AsVersionsBOper oper,
            AsVersion versionForDownload,
            bool isFullDbf)
        {
            oper.UpdateVersionStartDownload(versionForDownload, isFullDbf);
            tasks.Add(StartDownloadAsync( versionForDownload));
        }

        /// <summary>
        /// Starts the download asynchronous.
        /// </summary>
        /// <param name="versionForDownload">The version for download.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        private async Task StartDownloadAsync(AsVersion versionForDownload)
        {
            string urlForDownload = string.Empty;
            string filePath = string.Empty;
            try
            {
                urlForDownload = versionForDownload.IsDownloadedFiasFullDbf != null
                    ? versionForDownload.FiasFullDbfUrl
                    : (versionForDownload.IsDownloadedFiasDeltaDbf != null
                        ? versionForDownload.FiasDeltaDbfUrl
                        : null);

                if (urlForDownload.IsNullOrEmpty())
                    throw new Exception($"urlForDownload is null");

                if (!FakeReplaceUrlForDownload.IsNullOrEmpty())
                    urlForDownload = FakeReplaceUrlForDownload;

                filePath = _folderManager.GetFilePathByVersion(
                    versionForDownload,
                    _folderManager.GetFileNameFromUrl(urlForDownload));

                var webClient = new AddrSysWebClient();
                webClient.DownloadProgressChanged += (sender, args) => { DownloadProgressChanged(_logger, args); };

                _logger.Info($"Start download {urlForDownload} to {filePath}");

                if (File.Exists(filePath))
                {
                    var fi = new FileInfo(filePath);
                    _logger.Info($"File {filePath} exists {fi.Length}");

                    webClient.ResumeFileRange = (int)fi.Length;
                }

                if (urlForDownload == null)
                    throw new ArgumentNullException(nameof(urlForDownload));

                await webClient.DownloadFileTaskAsync(new Uri(urlForDownload), filePath);
                _logger.Info($"End download {urlForDownload} to {filePath}");

                _scope.UniverseFiasDb(
                    db => {
                        var oper = new AsVersionsBOper(db);
                        oper.UpdateVersionEndDownload(versionForDownload.Id, filePath);
                    });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error download {urlForDownload} to {filePath}: {ex.Message}");
                //throw; шаг загрузки не удался... В самом шаге это будет отмечено
            }
        }

        /// <summary>
        /// Downloads the progress changed.
        /// </summary>
        /// <param name="logger">The log step.</param>
        /// <param name="args">The <see cref="DownloadProgressChangedEventArgs"/> instance containing the event data.</param>
        private void DownloadProgressChanged(EventLogger logger, DownloadProgressChangedEventArgs args)
        {
            lock (logger)
                if (args.TotalBytesToReceive == args.BytesReceived)
                {
                    var seconds = DateTime.Now.Second;
                    if (seconds % 15 == 0)
                    {
                        var bytesReceived = args.BytesReceived;
                        logger.Info($"Принято данных: {bytesReceived} MiB");

                        var totalMbReceived = args.TotalBytesToReceive / (1024 * 1024);
                        logger.Info($"Всего принято данных: {totalMbReceived} MiB");

                        var progressPercentage = args.ProgressPercentage;
                        logger.Info($"Процент загрузки: {progressPercentage} %");
                    }
                }
        }
    }
}