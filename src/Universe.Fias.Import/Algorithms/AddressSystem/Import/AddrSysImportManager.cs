using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Universe.CQRS.Dal.Commands;
using Universe.CQRS.Dal.Queries;
using Universe.CQRS.Extensions;
using Universe.CQRS.Infrastructure;
using Universe.Diagnostic.Logger;
using Universe.Fias.Core.Infrastructure;
using Universe.Fias.DataAccess.Models;
using Universe.Fias.DataContracts.Consts;
using Universe.Fias.Import.Dal.Queries;
using Universe.Helpers.Extensions;
using Universe.Types;

namespace Universe.Fias.Import.Algorithms.AddressSystem.Import
{
    /// <summary>
    /// Address system import manager.
    /// </summary>
    /// <seealso cref="DisposableObject"/>
    public partial class AddrSysImportManager : DisposableObject
    {
        private readonly AddrSysFolderManager _folderManager;

        private readonly UniverseFiasScope _scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddrSysImportManager"/> class.
        /// </summary>
        /// <param name="scope">The CTX.</param>
        /// <param name="folderManager">The folder manager.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Ctx
        /// or
        /// folderManager
        /// </exception>
        public AddrSysImportManager([NotNull] UniverseFiasScope scope, [NotNull] AddrSysFolderManager folderManager)
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
        /// <param name="importContext">The import context.</param>
        /// <returns></returns>
        public Queue<AddrSysImportManager.GroupTaskImport> BuildTasks(List<string> filesNamesSrc, ImportContext importContext)
        {
            var filesNames = new List<string>(filesNamesSrc);
            var queueTasks = new Queue<AddrSysImportManager.GroupTaskImport>();

            foreach (var groupTaskImportDef in ImportTasksDef)
            {
                var tasks = new List<AddrSysImportManager.FileTaskImport>();
                foreach (var fileTaskImportDef in groupTaskImportDef.Tasks)
                {
                    if (!_scope.Settings.AllowedHousesImport && fileTaskImportDef.FilePattern.StartsWith(DbfFilePatterns.HOUSE))
                        continue;

                    var filesNamesByPattern = GetFilesNamesByPattern(fileTaskImportDef.FilePattern, filesNames);

                    foreach (var filesName in filesNamesByPattern)
                    {
                        tasks.Add(
                            new AddrSysImportManager.FileTaskImport(fileTaskImportDef, importContext) {
                                File = filesName
                            });
                    }
                }

                if (tasks.Count > 0)
                    queueTasks.Enqueue(
                        new AddrSysImportManager.GroupTaskImport(groupTaskImportDef, importContext) {
                            ImportType = groupTaskImportDef.ImportType,
                            Tasks = tasks.OrderBy(_ => _.File).ToList()
                        });
            }

            return queueTasks;
        }

        /// <summary>
        ///     Import new version.
        /// </summary>
        /// <param name="logStep">The log step.</param>
        /// <exception cref="System.InvalidOperationException">
        /// versionForImport.FiasDbfFolderPath.IsNullOrEmpty() and versionForImport.FiasDbfArchivePath.IsNullOrEmpty()
        /// </exception>
        public void Import(EventLogger logStep)
        {
            List<AsVersion> versionsForImport = null;
            _scope.UniverseFiasDb(
                db => {
                    var verOper = new AsVersionsBOper(db);
                    versionsForImport = verOper.GetVersionsForImport();
                });

            Debug.Assert(versionsForImport != null, "versionsForImport != null");
            //logStep.Stat.VersionsForImport = versionsForImport;
            if (versionsForImport.Count == 0)
            {
                logStep.Info("No version to import. Step skipped.");
                //logStep.SetFinalStatus(LogStepStatusEnum.StepSkipped);
                return;
            }

            try
            {
                foreach (var versionForImport in versionsForImport)
                {
                    try
                    {
                        if (versionForImport.FiasDbfFolderPath.IsNullOrEmpty() && versionForImport.FiasDbfArchivePath.IsNullOrEmpty())
                            throw new InvalidOperationException(
                                "versionForImport.FiasDbfFolderPath.IsNullOrEmpty() && versionForImport.FiasDbfArchivePath.IsNullOrEmpty()");

                        var fiasCsvFolderPath = !versionForImport.FiasCsvFolderPath.IsNullOrEmpty()
                            ? versionForImport.FiasCsvFolderPath
                            : !_scope.Settings.AddrSysConvertedCsvFilesPath.IsNullOrEmpty()
                                ? _scope.Settings.AddrSysConvertedCsvFilesPath
                                : throw new ArgumentException("Не указана директория с преобразованными из DBF CSV-файлами для импорта. " +
                                                              "Укажите директорию в приложении импорта и запустите приложения скачивания и преобразования DBF-файлов. ");

                        if (!Directory.Exists(fiasCsvFolderPath))
                            Directory.CreateDirectory(fiasCsvFolderPath);

                        var files = Directory.GetFiles(fiasCsvFolderPath);
                        if (files.Length == 0)
                        {
                            throw new ArgumentException(
                                $"В директории {fiasCsvFolderPath} не обнаружено ну одного преобразованного из DBF CSV-файла для импорта. " +
                                "Запустите приложение скачивания и преобразования DBF-файлов и при необходимости укажите в приложении импорта директорию, куда были сохранены CSV-файлы.");
                        }

                        versionForImport.FiasCsvFolderPath = fiasCsvFolderPath;
                        ProcessCsvFolder(logStep, versionForImport);

                        //if (!versionForImport.FiasDbfArchivePath.IsNullOrEmpty())
                        //    UnpackArchive(logStep, versionForImport);
                        //else
                        //    Import(versionForImport);
                    }
                    catch (Exception ex)
                    {
                        logStep.Error(ex, ex.Message);
                        throw;
                    }
                }
            }
            finally
            {
                PostProcessing(logStep);
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

        private void Import(AsVersion versionForImport)
        {
            throw new NotImplementedException();
        }

        private void PostProcessing(EventLogger logStep)
        {
            logStep.Info("Запуск постобработки...");

            SetThereAreStreets(logStep);
            SetRegionCodeHouse(logStep);
        }

        private void SetThereAreStreets(EventLogger logStep)
        {
            logStep.Info("Заполнение ThereAreStreets...");

            //using (
            //    var logStep = parentLogStep.CreateChild(
            //        "Update types set there are streets",
            //        new object()))
            //    logStep.Run(
            //        new {
            //            logStep
            //        },
            //        action: ctx => {

            try
            {
                // Выполняем содержимое бывшей хранимой процедуры AsSetThereAreStreets
                var dbName = _scope.DbCtx.Database.Connection.Database;
                _scope.GetQuery<UniverseFiasDirectSqlQuery<object>>().Execute(
                    @$"USE [{dbName}]; 
                        DECLARE @updated table(  
                        Code varchar(4) NOT NULL
                        );  

	                    update aot
	                    set aot.ThereAreStreets = 1
	                    output inserted.Code into @updated
	                    from dbo.AsAddrObjTypes aot where exists (
	                    select 1 from dbo.AsAddrObjs ao 
	                    join dbo.AsAddrObjs c on c.ParentAoId = ao.AoId and c.AoLevel = 7 and c.LiveStatusId = 1
	                    where ao.TypeCode = aot.Code and ao.LiveStatusId = 1);

	                    update aot
	                    set aot.ThereAreStreets = 0
	                    from dbo.AsAddrObjTypes aot 
	                    where not exists(select 1 from @updated up where up.Code = aot.Code);
                 ",
                    60 * 60);
            }
            catch (Exception ex)
            {
                logStep.Error(ex, ex.Message);
            }

            logStep.Info("Дозаполнение ThereAreStreets у некоторых типов адресных объектов...");

            // Дополнительно указываем у некоторых типов ThereAreStreets = true, т.к они могут иметь улицы по логике вещей
            var addrObjTypesDbs = _scope.GetQuery<GetEntitiesQuery<AsAddrObjType>>().Execute(
                    EntityReqHelper.GetAnyReq(MetaInfoHelper.FieldMap(
                            MetaInfoHelper.MapRule<AsAddrObjType>(nameof(AsAddrObjType.Id), x => x.Id)),
                        allItemsAsOnePage: true))
                .Items;

            foreach (var addrObjTypesDb in addrObjTypesDbs)
            {
                if (addrObjTypesDb.Name.PrepareToCompare() == "поселок городского типа" ||
                    addrObjTypesDb.Name.PrepareToCompare() == "деревня")
                {
                    addrObjTypesDb.ThereAreStreets = true;
                }
            }

            _scope.GetCommand<UpdateEntitiesCommand<AsAddrObjType>>().Execute(addrObjTypesDbs);

            logStep.Info("ThereAreStreets заполнен!");
            //        });
        }

        private void SetRegionCodeHouse(EventLogger logStep)
        {
            logStep.Info("Заполнение RegionCode в таблице AsHouses...");

            //using (
            //    var logStep = parentLogStep.CreateChild(
            //        "Update types set there are streets",
            //        new object()))
            //    logStep.Run(
            //        new {
            //            logStep
            //        },
            //        action: ctx => {

            // Выполняем содержимое SQL-скрипта из SetRegionCodeHouse
            var dbName = _scope.DbCtx.Database.Connection.Database;
            _scope.GetQuery<UniverseFiasDirectSqlQuery<AsHouse>>().Execute(
                @$"USE [{dbName}]; 
                    update dbo.AsHouses
                    set RegionCode = SUBSTRING(ao.PlainCode, 1, 2)
                    --select top 10 ao.PlainCode, SUBSTRING(ao.PlainCode, 1, 2), *
                    from dbo.AsHouses as h
                        left join dbo.AsAddrObjs ao on ao.AoId = h.AoId and ao.NextId is null
                    where h.RegionCode is null --and ao.AoId is null
                    ;

                    update dbo.AsHouses
                    set RegionCode = (select top 1 SUBSTRING(ao.PlainCode, 1, 2) from dbo.AsAddrObjs ao where ao.AoId = AsHouses.AoId and ao.NextId is null)
                    where RegionCode is null;

                    select top 100 h.RegionCode, ao.PlainCode, SUBSTRING(ao.PlainCode, 1, 2), *
                    from dbo.AsHouses as h
                        left join dbo.AsAddrObjs ao on ao.AoId = h.AoId and ao.NextId is null
                    where h.RegionCode != SUBSTRING(ao.PlainCode, 1, 2);

                    select count( *)
                    --delete
                    from dbo.AsHouses 
                    where RegionCode is null;

                    select count(*)
                    --delete
                    from dbo.AsHouses h
                        left join dbo.AsAddrObjs ao on ao.AoId = h.AoId 
                    where ao.AoId is null;
                 ",
                60 * 60);

            logStep.Info("RegionCode в таблице AsHouses заполнен!");
            //        });
        }

        private void UnpackArchive(EventLogger logStep, AsVersion versionForImport)
        {
            //using (
            //    var logStep = parentLogStep.CreateChild(
            //        $"Unpack archive {versionForImport.VersionId} ({versionForImport.TextVersion}) {versionForImport.FiasDbfArchivePath}",
            //        new AddrSysUnpackArchiveVersionStat {
            //            Version = versionForImport
            //        }))
            //    logStep.Run(
            //        new {
            //            logStep
            //        },
            //        action: ctx => {

            var sevenZipArchive = _scope.Factory.Create7ZipArchive();

            if (versionForImport.FiasDbfArchivePath.IsNullOrEmpty())
                throw new InvalidOperationException("versionForImport.FiasDbfArchivePath.IsNullOrEmpty()");

            if (versionForImport.FiasDbfFolderPath.IsNullOrEmpty())
            {
                versionForImport.FiasDbfFolderPath =
                    _folderManager.GetOrCreateContentArchiveFolderForVersion(versionForImport);
                _scope.UniverseFiasDb(
                    db =>
                    {
                        var verOper = new AsVersionsBOper(db);
                        verOper.UpdateVersion(versionForImport);
                    });
            }

            var filesNames = sevenZipArchive.ListContents(versionForImport.FiasDbfArchivePath);
            //logStep.Stat.ArchiveFilesNames = filesNames;

            var fiasCsvFolderPath = !versionForImport.FiasCsvFolderPath.IsNullOrEmpty()
                ? versionForImport.FiasCsvFolderPath
                : !_scope.Settings.AddrSysConvertedCsvFilesPath.IsNullOrEmpty()
                    ? _scope.Settings.AddrSysConvertedCsvFilesPath
                    : throw new ArgumentException(
                        "Не указана директория с преобразованными из DBF CSV-файлами для импорта. " +
                        "Укажите директорию в приложении импорта и запустите приложения скачивания и преобразования DBF-файлов. ");

            if (!Directory.Exists(fiasCsvFolderPath))
                Directory.CreateDirectory(fiasCsvFolderPath);

            var files = Directory.GetFiles(fiasCsvFolderPath);
            if (files.Length == 0)
            {
                throw new ArgumentException(
                    $"В директории {fiasCsvFolderPath} не обнаружено ну одного преобразованного из DBF CSV-файла для импорта. " +
                    "Запустите приложение скачивания и преобразования DBF-файлов и при необходимости укажите в приложении импорта директорию, куда были сохранены CSV-файлы.");
            }

            var importContext = new ImportContext
            {
                Scope = _scope,
                FiasDbfArchivePath = versionForImport.FiasDbfArchivePath,
                FiasDbfFolderPath = versionForImport.FiasDbfFolderPath,
                FiasCsvFolderPath = fiasCsvFolderPath,
                IsFiasFullDbf = versionForImport.IsDownloadedFiasFullDbf ?? false,
                VersionId = versionForImport.Id
            };

            var queueTasks = BuildTasks(filesNames, importContext);

            //logStep.Stat.FilesForImport = queueTasks.SelectMany(_ => _.Tasks.Select(t => t.File)).ToList();

            if (queueTasks.Count == 0)
            {
                logStep.Info("No files to import. Step skipped.");
                //logStep.SetFinalStatus(LogStepStatusEnum.StepSkipped);
                return;
            }

            var dbName = importContext.Scope.DbCtx.Database.Connection.Database;

            //var lockTimeout = 30 * 60 * 1000;
            //// Установка LockTimeout
            //importContext.Scope.GetQuery<DirectSqlQuery<AsAddrObj>>().Execute(
            //    @$"USE [{dbName}]; 
            //       SET LOCK_TIMEOUT {lockTimeout};  
            //       SELECT @@LOCK_TIMEOUT AS [Lock Timeout];  
            //      "
            //);

            // Устраняется deadlock
            importContext.Scope.GetQuery<DirectSqlQuery<AsAddrObj>>().Execute(
                @$"exec sp_updatestats
                   dbcc freeproccache
                  "
            );

            // Удаление всех записей из таблицы AsAddrObjs
            importContext.Scope.GetQuery<DirectSqlQuery<AsAddrObj>>().Execute(
                @$"USE [{dbName}]; 
                                       DELETE FROM [dbo].[AsAddrObjs];
                                       TRUNCATE TABLE [dbo].[AsAddrObjs];
                                       SET IDENTITY_INSERT [dbo].[AsAddrObjs] ON 
                                       DBCC CHECKIDENT('[dbo].[AsAddrObjs]', RESEED, 1);
                                       SET IDENTITY_INSERT [dbo].[AsAddrObjs] OFF

                                       SELECT [Id],[AoId],[ParentAoId],[DivType],[AoLevel],
                                              [TypeShortName],[Name],[OffName],[Code],[PlainCode],
                                              [PostalCode],[OKATO],[OKTMO],[LiveStatusId],[UpdateDate],
                                              [StartDate],[EndDate],[TypeCode],[AoCode],[ActStatusId],
                                              [CentStatusId],[OperStatusId],[NextId],[PrevId],[TypeId]  
                                         FROM [dbo].[AsAddrObjs]
                                       "
            );

            while (queueTasks.Count > 0)
            {
                try
                {
                    var groupTaskImport = queueTasks.Dequeue();
                    var task = groupTaskImport.RunAsync(logStep, _scope.Settings);
                    task.Wait();

                    if (task.IsFaulted)
                        throw new Exception($"Group task import error: {task.Exception?.Message}", task.Exception);
                }
                catch (Exception ex)
                {
                    logStep.Error(ex, ex.Message);
                    //logStep.LogError(ex.Message, ex);
                }
            }

            //if (!logStep.IsError)
            _scope.UniverseFiasDb(
                db =>
                {
                    var verOper = new AsVersionsBOper(db);
                    versionForImport.IsProcesed = true;
                    verOper.UpdateVersion(versionForImport);
                    logStep.Info($"{versionForImport.TextVersion} set IsProcesed");
                });
            //});
        }

        private void ProcessCsvFolder(EventLogger logStep, AsVersion versionForImport)
        {
            //using (
            //    var logStep = parentLogStep.CreateChild(
            //        $"Unpack archive {versionForImport.VersionId} ({versionForImport.TextVersion}) {versionForImport.FiasDbfArchivePath}",
            //        new AddrSysUnpackArchiveVersionStat {
            //            Version = versionForImport
            //        }))
            //    logStep.Run(
            //        new {
            //            logStep
            //        },
            //        action: ctx => {

            if (versionForImport.FiasCsvFolderPath.IsNullOrEmpty())
                throw new InvalidOperationException("versionForImport.FiasDbfArchivePath.IsNullOrEmpty()");

            if (!Directory.Exists(versionForImport.FiasCsvFolderPath))
                Directory.CreateDirectory(versionForImport.FiasCsvFolderPath);

            var filesNames = Directory.GetFiles(versionForImport.FiasCsvFolderPath).ToList();
            if (filesNames.Count == 0)
            {
                throw new ArgumentException(
                    $"В директории {versionForImport.FiasCsvFolderPath} не обнаружено ну одного преобразованного из DBF CSV-файла для импорта. " +
                    "Запустите приложение скачивания и преобразования DBF-файлов и при необходимости укажите в приложении импорта директорию, куда были сохранены CSV-файлы.");
            }

            //logStep.Stat.ArchiveFilesNames = filesNames;

            var importContext = new ImportContext
            {
                Scope = _scope,
                FiasDbfArchivePath = versionForImport.FiasDbfArchivePath,
                FiasDbfFolderPath = versionForImport.FiasDbfFolderPath,
                FiasCsvFolderPath = versionForImport.FiasCsvFolderPath,
                IsFiasFullDbf = versionForImport.IsDownloadedFiasFullDbf ?? false,
                VersionId = versionForImport.Id
            };

            var queueTasks = BuildTasks(filesNames, importContext);

            //logStep.Stat.FilesForImport = queueTasks.SelectMany(_ => _.Tasks.Select(t => t.File)).ToList();

            if (queueTasks.Count == 0)
            {
                logStep.Info("No files to import. Step skipped.");
                //logStep.SetFinalStatus(LogStepStatusEnum.StepSkipped);
                return;
            }

            var dbName = importContext.Scope.DbCtx.Database.Connection.Database;
            // Удаление всех записей из таблицы AsAddrObjs
            importContext.Scope.GetQuery<UniverseFiasDirectSqlQuery<AsAddrObj>>().Execute(
                @$"USE [{dbName}]; 
                   DELETE FROM [dbo].[AsAddrObjs];
                   TRUNCATE TABLE [dbo].[AsAddrObjs];
                   SET IDENTITY_INSERT [dbo].[AsAddrObjs] ON 
                   DBCC CHECKIDENT('[dbo].[AsAddrObjs]', RESEED, 1);
                   SET IDENTITY_INSERT [dbo].[AsAddrObjs] OFF

                   SELECT [Id],[AoId],[ParentAoId],[DivType],[AoLevel],
                          [TypeShortName],[Name],[OffName],[Code],[PlainCode],
                          [PostalCode],[OKATO],[OKTMO],[LiveStatusId],[UpdateDate],
                          [StartDate],[EndDate],[TypeCode],[AoCode],[ActStatusId],
                          [CentStatusId],[OperStatusId],[NextId],[PrevId],[TypeId]  
                     FROM [dbo].[AsAddrObjs]
                ",
                60 * 60
            );

            // Удаление всех записей из таблицы AsHouses
            importContext.Scope.GetQuery<UniverseFiasDirectSqlQuery<AsHouse>>().Execute(
                @$"USE [{dbName}]; 
                   DELETE FROM [dbo].[AsHouses];
                   TRUNCATE TABLE [dbo].[AsHouses];
                   SET IDENTITY_INSERT [dbo].[AsHouses] ON 
                   DBCC CHECKIDENT('[dbo].[AsHouses]', RESEED, 1);
                   SET IDENTITY_INSERT [dbo].[AsHouses] OFF

                   SELECT [Id],[HouseId],[AoId],[DivType],[HouseNum],
                          [BuildNum],[StrucNum],[PostalCode],[OKATO],
                          [OKTMO],[UpdateDate],[StartDate],[EndDate],
                          [RegionCode],[EstStatusId],[StrStatusId]  
                     FROM [dbo].[AsHouses]
                ",
                60 * 60
            );

            while (queueTasks.Count > 0)
            {
                try
                {
                    var groupTaskImport = queueTasks.Dequeue();
                    var task = groupTaskImport.RunAsync(logStep, _scope.Settings);
                    task.Wait();

                    if (task.IsFaulted)
                        throw new Exception($"Group task import error: {task.Exception?.Message}", task.Exception);
                }
                catch (Exception ex)
                {
                    logStep.Error(ex, ex.Message);
                    //logStep.LogError(ex.Message, ex);
                }
            }

            //if (!logStep.IsError)
            _scope.UniverseFiasDb(
                db => {
                    var verOper = new AsVersionsBOper(db);
                    versionForImport.IsProcesed = true;
                    verOper.UpdateVersion(versionForImport);
                    logStep.Info($"{versionForImport.TextVersion} set IsProcesed");
                });
            //});
        }
    }
}