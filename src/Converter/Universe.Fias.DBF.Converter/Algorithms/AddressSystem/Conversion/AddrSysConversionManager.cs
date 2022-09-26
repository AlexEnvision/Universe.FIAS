using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Universe.Diagnostic.Logger;
using Universe.Fias.Core.Infrastructure;
using Universe.Fias.DataAccess.Models;
using Universe.Helpers.Extensions;
using Universe.Types;
using Universe.Types.Collection;

namespace Universe.Fias.DBF.Converter.x64.Algorithms.AddressSystem.Conversion
{
    /// <summary>
    /// Address system conversion manager.
    /// </summary>
    /// <seealso cref="DisposableObject"/>
    public partial class AddrSysConversionManager : DisposableObject
    {
        private readonly AddrSysFolderManager _folderManager;

        private readonly UniverseFiasScope _scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="Conversion.AddrSysConversionManager"/> class.
        /// </summary>
        /// <param name="scope">The CTX.</param>
        /// <param name="folderManager">The folder manager.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Ctx
        /// or
        /// folderManager
        /// </exception>
        public AddrSysConversionManager([NotNull] UniverseFiasScope scope, [NotNull] AddrSysFolderManager folderManager)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (folderManager == null)
                throw new ArgumentNullException(nameof(folderManager));

            _scope = scope;
            _folderManager = folderManager;
        }

        /// <summary>
        /// Builds the tasks.
        /// </summary>
        /// <param name="filesNamesSrc">The files names source.</param>
        /// <param name="conversionContext">The conversion context.</param>
        /// <returns></returns>
        public Queue<GroupTaskConversion> BuildTasks(List<string> filesNamesSrc, ConversionContext conversionContext)
        {
            var filesNames = new List<string>(filesNamesSrc);
            var queueTasks = new Queue<GroupTaskConversion>();

            foreach (var groupTaskconversionDef in ConversionTasksDef)
            {
                var tasks = new List<FileTaskconversion>();
                foreach (var fileTaskconversionDef in groupTaskconversionDef.Tasks)
                {
                    var filesNamesByPattern = GetFilesNamesByPattern(fileTaskconversionDef.FilePattern, filesNames);

                    foreach (var filesName in filesNamesByPattern)
                    {
                        tasks.Add(
                            new FileTaskconversion(fileTaskconversionDef, conversionContext) {
                                File = filesName
                            });
                    }
                }

                if (tasks.Count > 0)
                    queueTasks.Enqueue(
                        new GroupTaskConversion(groupTaskconversionDef, conversionContext) {
                            ConversionType = groupTaskconversionDef.ConversionType,
                            Tasks = tasks.OrderBy(_ => _.File).ToList()
                        });
            }

            return queueTasks;
        }

        /// <summary>
        ///     Conversion new version.
        /// </summary>
        /// <param name="logStep">The log step.</param>
        /// <exception cref="System.InvalidOperationException">
        /// versionForconversion.FiasDbfFolderPath.IsNullOrEmpty() and versionForconversion.FiasDbfArchivePath.IsNullOrEmpty()
        /// </exception>
        public void Transform(EventLogger logStep)
        {
            List<AsVersion> versionsForConversion = null;
            _scope.UniverseFiasDb(
                db => {
                    var verOper = new AsVersionsBOper(db);
                    versionsForConversion = verOper.GetVersionsForConversion();
                });

            Debug.Assert(versionsForConversion != null, "versionsForConversion != null");
            if (versionsForConversion.Count == 0)
            {
                logStep.Info("No version to conversion. Step skipped.");
                return;
            }

            foreach (var versionForConversion in versionsForConversion)
            {
                try
                {
                    logStep.Info($"Process version for conversion: '{versionForConversion.TextVersion}'...");

                    if (versionForConversion.FiasDbfFolderPath.IsNullOrEmpty() && versionForConversion.FiasDbfArchivePath.IsNullOrEmpty())
                        throw new InvalidOperationException(
                            "versionForconversion.FiasDbfFolderPath.IsNullOrEmpty() && versionForconversion.FiasDbfArchivePath.IsNullOrEmpty()");

                    if (!versionForConversion.FiasDbfArchivePath.IsNullOrEmpty())
                        UnpackArchive(logStep, versionForConversion);
                    else
                        Convert(versionForConversion);
                }
                catch (Exception ex)
                {
                    logStep.Error(ex, ex.Message);
                    throw;
                }
            }
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

        internal List<string> GetFilesNamesByPattern(string filePattern, List<string> filesNames)
        {
            var result = new List<string>();
            for (var i = filesNames.Count - 1; i >= 0; i--)
            {
                var fileName = filesNames[i];

                if (Regex.IsMatch(fileName, filePattern, RegexOptions.IgnoreCase))
                {
                    result.Add(fileName);
                    filesNames.RemoveAt(i);
                }
            }

            return result;
        }

        private void Convert(AsVersion versionForConvert)
        {
            throw new NotImplementedException();
        }

        private void UnpackArchive(EventLogger logStep, AsVersion versionForConvert)
        {
            var sevenZipArchive = _scope.Factory.Create7ZipArchive();

            if (versionForConvert.FiasDbfArchivePath.IsNullOrEmpty())
                throw new InvalidOperationException("versionForconversion.FiasDbfArchivePath.IsNullOrEmpty()");

            if (versionForConvert.FiasDbfFolderPath.IsNullOrEmpty())
            {
                versionForConvert.FiasDbfFolderPath =
                    _folderManager.GetOrCreateContentArchiveFolderForVersion(versionForConvert);
                _scope.UniverseFiasDb(
                    db =>
                    {
                        var verOper = new AsVersionsBOper(db);
                        verOper.UpdateVersion(versionForConvert);
                    });
            }

            var filesNames = sevenZipArchive.ListContents(versionForConvert.FiasDbfArchivePath);
            var fiasCsvFolderPath = !versionForConvert.FiasCsvFolderPath.IsNullOrEmpty()
                ? versionForConvert.FiasCsvFolderPath
                : !_scope.Settings.AddrSysConvertedCsvFilesPath.IsNullOrEmpty()
                    ? _scope.Settings.AddrSysConvertedCsvFilesPath
                    : throw new ArgumentException(
                        "Не указана директория для сохранения преобразованных из DBF CSV-файлов для последующего импорта. " +
                        "Укажите директорию в приложении, либо запустите скачивание архива DBF-файлов с nalog.ru.");

            if (!Directory.Exists(fiasCsvFolderPath))
                Directory.CreateDirectory(fiasCsvFolderPath);

            var files = Directory.GetFiles(fiasCsvFolderPath);
            if (files.Length > 0)
            {
                logStep.Info($"Директория {fiasCsvFolderPath} не пуста - удаляются все файлы, находящиеся в ней...");

                var filenames = new MatList<string>();
                foreach (var file in files)
                {
                    var filename = Path.GetFileName(file);
                    filenames += filename;
                    File.Delete(file);
                }
                logStep.Info($"Удалено файлов: {files.Length}. Названия: {string.Join(",", filenames)}.");
            }

            var conversionContext = new ConversionContext
            {
                Scope = _scope,
                FiasDbfArchivePath = versionForConvert.FiasDbfArchivePath,
                FiasDbfFolderPath = versionForConvert.FiasDbfFolderPath,
                FiasCsvFolderPath = fiasCsvFolderPath,
                IsFiasFullDbf = versionForConvert.IsDownloadedFiasFullDbf ?? false,
                VersionId = versionForConvert.Id
            };

            var queueTasks = BuildTasks(filesNames, conversionContext);

            var filesForConversion = queueTasks.SelectMany(_ => _.Tasks.Select(t => t.File)).ToList();
            var filesForConversionSfy = string.Join(", ", filesForConversion);
            logStep.Info($"Files to conversion: {filesForConversionSfy}");

            if (queueTasks.Count == 0)
            {
                logStep.Info("No files to conversion. Step skipped.");
                return;
            }

            while (queueTasks.Count > 0)
            {
                try
                {
                    var groupTaskConversion = queueTasks.Dequeue();
                    var task = groupTaskConversion.RunAsync(logStep, _scope.Settings);
                    task.Wait();

                    if (task.IsFaulted)
                        throw new Exception($"Group task conversion error: {task.Exception?.Message}", task.Exception);
                }
                catch (Exception ex)
                {
                    logStep.Error(ex, ex.Message);
                }
            }
        }
    }
}