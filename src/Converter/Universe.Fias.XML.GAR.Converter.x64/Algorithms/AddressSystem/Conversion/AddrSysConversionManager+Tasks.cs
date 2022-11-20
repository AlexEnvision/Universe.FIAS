using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using Universe.Diagnostic.Logger;
using Universe.Fias.Core.Infrastructure;
using Universe.Fias.IO.Compression;
using Universe.Fias.XML.GAR.Converter.x64.Algorithms.AddressSystem.Conversion.XML;

namespace Universe.Fias.XML.GAR.Converter.x64.Algorithms.AddressSystem.Conversion
{
    public partial class AddrSysConversionManager
    {
        /// <summary>
        /// The conversion tasks definition.
        /// </summary>
        internal readonly List<GroupTaskConversionDef> ConversionTasksDef = new List<GroupTaskConversionDef> {
            new GroupTaskConversionDef {
                ConversionType = GroupTaskConversionDef.ProcessingTypes.Parallel,
                Tasks = new List<FileTaskConversionDef> {
                    new FileTaskConversionDef {
                        FilePattern = "ACTSTAT.Xml",
                        SqlconversionStoreProc = "dbo.AsconversionActStatuses",
                        Insert = true,
                        Update = true,
                        DeleteIfMatched = false,
                        DeleteIfNotMatchedBySource = false,
                        MappingFields = new List<KeyValuePair<string, object>> {
                            new KeyValuePair<string, object>("ACTSTATID", "Id"),
                            new KeyValuePair<string, object>("NAME", "Name"),
                            new KeyValuePair<string, object>("null", "ShortName")
                        }
                    },
                    new FileTaskConversionDef {
                        FilePattern = "CENTERST.Xml",
                        SqlconversionStoreProc = "dbo.AsconversionCenterStatuses",
                        Insert = true,
                        Update = true,
                        DeleteIfMatched = false,
                        DeleteIfNotMatchedBySource = false,
                        MappingFields = new List<KeyValuePair<string, object>> {
                            new KeyValuePair<string, object>("CENTERSTID", "Id"),
                            new KeyValuePair<string, object>("NAME", "Name"),
                            new KeyValuePair<string, object>("null", "ShortName")
                        }
                    },
                    new FileTaskConversionDef {
                        FilePattern = "ESTSTAT.Xml",
                        SqlconversionStoreProc = "dbo.AsconversionEstStatuses",
                        Insert = true,
                        Update = true,
                        DeleteIfMatched = false,
                        DeleteIfNotMatchedBySource = false,
                        MappingFields = new List<KeyValuePair<string, object>> {
                            new KeyValuePair<string, object>("ESTSTATID", "Id"),
                            new KeyValuePair<string, object>("NAME", "Name"),
                            new KeyValuePair<string, object>("SHORTNAME", "ShortName")
                        }
                    },
                    new FileTaskConversionDef {
                        FilePattern = "OPERSTAT.Xml",
                        SqlconversionStoreProc = "dbo.AsconversionOperStatuses",
                        Insert = true,
                        Update = true,
                        DeleteIfMatched = false,
                        DeleteIfNotMatchedBySource = false,
                        MappingFields = new List<KeyValuePair<string, object>> {
                            new KeyValuePair<string, object>("OPERSTATID", "Id"),
                            new KeyValuePair<string, object>("NAME", "Name"),
                            new KeyValuePair<string, object>("null", "ShortName")
                        }
                    },
                    new FileTaskConversionDef {
                        FilePattern = "SOCRBASE.Xml",
                        SqlconversionStoreProc = "dbo.AsconversionAddrObjTypes",
                        Insert = true,
                        Update = true,
                        DeleteIfMatched = false,
                        DeleteIfNotMatchedBySource = false,
                        MappingFields = new List<KeyValuePair<string, object>> {
                            new KeyValuePair<string, object>("KOD_T_ST", "Code"),
                            new KeyValuePair<string, object>("[LEVEL]", "[Level]"),
                            new KeyValuePair<string, object>("SOCRNAME", "Name"),
                            new KeyValuePair<string, object>("SCNAME", "ShortName")
                        },
                        WhereFilter = "KOD_T_ST <> '0'"
                    },
                    new FileTaskConversionDef {
                        FilePattern = "STRSTAT.Xml",
                        SqlconversionStoreProc = "dbo.AsconversionStrStatuses",
                        Insert = true,
                        Update = true,
                        DeleteIfMatched = false,
                        DeleteIfNotMatchedBySource = false,
                        MappingFields = new List<KeyValuePair<string, object>> {
                            new KeyValuePair<string, object>("STRSTATID", "Id"),
                            new KeyValuePair<string, object>("NAME", "Name"),
                            new KeyValuePair<string, object>("SHORTNAME", "ShortName")
                        }
                    }
                }
            },
            new GroupTaskConversionDef {
                ConversionType = GroupTaskConversionDef.ProcessingTypes.Parallel,
                Tasks = new List<FileTaskConversionDef> {
                    new FileTaskConversionDef {
                        FilePattern = "ADDROB[0-9]{2}.Xml",
                        SqlconversionStoreProc = "dbo.AsconversionAddrObjs",
                        IsUseRegionCode = true,
                        Insert = true,
                        Update = true,
                        DeleteIfMatched = false,
                        DeleteIfNotMatchedBySource = false,
                        MappingFields = new List<KeyValuePair<string, object>> {
                            new KeyValuePair<string, object>("AOID", new SqlMetaData("Id", SqlDbType.UniqueIdentifier)),
                            new KeyValuePair<string, object>("PREVID", new SqlMetaData("PrevId", SqlDbType.UniqueIdentifier)),
                            new KeyValuePair<string, object>("NEXTID", new SqlMetaData("NextId", SqlDbType.UniqueIdentifier)),
                            new KeyValuePair<string, object>("AOGUID", new SqlMetaData("AoId", SqlDbType.UniqueIdentifier)),
                            new KeyValuePair<string, object>("PARENTGUID", new SqlMetaData("ParentAoId", SqlDbType.UniqueIdentifier)),
                            new KeyValuePair<string, object>("DIVTYPE", new SqlMetaData("DivType", SqlDbType.Int)),
                            new KeyValuePair<string, object>("AOLEVEL", new SqlMetaData("AoLevel", SqlDbType.Int)),
                            new KeyValuePair<string, object>("SHORTNAME", new SqlMetaData("TypeShortName", SqlDbType.NVarChar, -1)),
                            new KeyValuePair<string, object>("FORMALNAME", new SqlMetaData("Name", SqlDbType.NVarChar, -1)),
                            new KeyValuePair<string, object>("OFFNAME", new SqlMetaData("OffName", SqlDbType.NVarChar, -1)),
                            new KeyValuePair<string, object>("CENTSTATUS", new SqlMetaData("CentStatusId", SqlDbType.Int)),
                            new KeyValuePair<string, object>("CODE", new SqlMetaData("Code", SqlDbType.NVarChar, -1)),
                            new KeyValuePair<string, object>("PLAINCODE", new SqlMetaData("PlainCode", SqlDbType.NVarChar, -1)),
                            new KeyValuePair<string, object>("POSTALCODE", new SqlMetaData("PostalCode", SqlDbType.NVarChar, -1)),
                            new KeyValuePair<string, object>("OKATO", new SqlMetaData("OKATO", SqlDbType.NVarChar, -1)),
                            new KeyValuePair<string, object>("OKTMO", new SqlMetaData("OKTMO", SqlDbType.NVarChar, -1)),
                            new KeyValuePair<string, object>("LIVESTATUS", new SqlMetaData("LiveStatusId", SqlDbType.Int)),
                            new KeyValuePair<string, object>("ACTSTATUS", new SqlMetaData("ActStatusId", SqlDbType.Int)),
                            new KeyValuePair<string, object>("OPERSTATUS", new SqlMetaData("OperStatusId", SqlDbType.Int)),
                            new KeyValuePair<string, object>("UPDATEDATE", new SqlMetaData("UpdateDate", SqlDbType.Date)),
                            new KeyValuePair<string, object>("STARTDATE", new SqlMetaData("StartDate", SqlDbType.Date)),
                            new KeyValuePair<string, object>("ENDDATE", new SqlMetaData("EndDate", SqlDbType.Date))

                            //new KeyValuePair<string, object>("null", "PrevId"),
                            //new KeyValuePair<string, object>("null", "NextId"),
                            //new KeyValuePair<string, object>("AOGUID", "AoId"),
                            //new KeyValuePair<string, object>("null", "ParentAoId"),
                            //new KeyValuePair<string, object>("null", "DivType"),
                            //new KeyValuePair<string, object>("null", "AoLevel"),
                            //new KeyValuePair<string, object>("null", "TypeShortName"),
                            //new KeyValuePair<string, object>("null", "Name"),
                            //new KeyValuePair<string, object>("null", "OffName"),
                            //new KeyValuePair<string, object>("null", "CentStatusId"),
                            //new KeyValuePair<string, object>("null", "Code"),
                            //new KeyValuePair<string, object>("null", "PlainCode"),
                            //new KeyValuePair<string, object>("null", "PostalCode"),
                            //new KeyValuePair<string, object>("null", "OKATO"),
                            //new KeyValuePair<string, object>("null", "OKTMO"),
                            //new KeyValuePair<string, object>("LIVESTATUS", "LiveStatusId"),
                            //new KeyValuePair<string, object>("null", "ActStatusId"),
                            //new KeyValuePair<string, object>("null", "OperStatusId"),
                            //new KeyValuePair<string, object>("null", "UpdateDate"),
                            //new KeyValuePair<string, object>("null", "StartDate"),
                            //new KeyValuePair<string, object>("ENDDATE", "EndDate")
                            /*AREACODE	AUTOCODE	CITYCODE	CURRSTATUS	IFNSFL	IFNSUL	PLACECODE	REGIONCODE	STREETCODE	TERRIFNSFL	
                         * TERRIFNSUL	CTARCODE	EXTRCODE	SEXTCODE	NORMDOC	PLANCODE	CADNUM*/
                        },
                        // WhereIsFiasFullXml = "LIVESTATUS = 1" - грузим все
                    }
                }
            },
            new GroupTaskConversionDef {
                ConversionType = GroupTaskConversionDef.ProcessingTypes.Parallel,
                Tasks = new List<FileTaskConversionDef> {
                    new FileTaskConversionDef {
                        FilePattern = "HOUSE[0-9]{2}.Xml",
                        SqlconversionStoreProc = "dbo.AsconversionHouses",
                        IsUseRegionCode = true,
                        Insert = true,
                        Update = true,
                        DeleteIfMatched = false,
                        DeleteIfNotMatchedBySource = false,
                        MappingFields = new List<KeyValuePair<string, object>> {
                            new KeyValuePair<string, object>("HOUSEID", new SqlMetaData("Id", SqlDbType.UniqueIdentifier)),
                            new KeyValuePair<string, object>("HOUSEGUID", new SqlMetaData("HouseId", SqlDbType.UniqueIdentifier)),
                            new KeyValuePair<string, object>("AOGUID", new SqlMetaData("AoId", SqlDbType.UniqueIdentifier)),
                            new KeyValuePair<string, object>("DIVTYPE", new SqlMetaData("DivType", SqlDbType.Int)),
                            new KeyValuePair<string, object>("HOUSENUM", new SqlMetaData("HouseNum", SqlDbType.NVarChar, -1)),
                            new KeyValuePair<string, object>("ESTSTATUS", new SqlMetaData("EstStatusId", SqlDbType.Int)),
                            new KeyValuePair<string, object>("BUILDNUM", new SqlMetaData("BuildNum", SqlDbType.NVarChar, -1)),
                            new KeyValuePair<string, object>("STRUCNUM", new SqlMetaData("StrucNum", SqlDbType.NVarChar, -1)),
                            new KeyValuePair<string, object>("STRSTATUS", new SqlMetaData("StrStatusId", SqlDbType.Int)),
                            new KeyValuePair<string, object>("POSTALCODE", new SqlMetaData("PostalCode", SqlDbType.NVarChar, -1)),
                            new KeyValuePair<string, object>("OKATO", new SqlMetaData("OKATO", SqlDbType.NVarChar, -1)),
                            new KeyValuePair<string, object>("OKTMO", new SqlMetaData("OKTMO", SqlDbType.NVarChar, -1)),
                            new KeyValuePair<string, object>("UPDATEDATE", new SqlMetaData("UpdateDate", SqlDbType.Date)),
                            new KeyValuePair<string, object>("STARTDATE", new SqlMetaData("StartDate", SqlDbType.Date)),
                            new KeyValuePair<string, object>("ENDDATE", new SqlMetaData("EndDate", SqlDbType.Date))
                            /*STATSTATUS	IFNSFL	IFNSUL	TERRIFNSFL	TERRIFNSUL	NORMDOC	COUNTER	CADNUM*/
                        },
                        WhereFilter = "(AOGUID is not null and AOGUID <> '')"
                        // WhereIsFiasFullXml = "DATE() between STARTDATE and ENDDATE"  - грузим все
                    }
                }
            },
            new GroupTaskConversionDef {
                ConversionType = GroupTaskConversionDef.ProcessingTypes.Serial,
                Tasks = new List<FileTaskConversionDef> {
                    new FileTaskConversionDef {
                        FilePattern = "DHOUSE.Xml",
                        SqlconversionStoreProc = "dbo.AsconversionDeleteHouses",
                        IsUseRegionCode = false,
                        Insert = false,
                        Update = false,
                        DeleteIfMatched = true,
                        DeleteIfNotMatchedBySource = false,
                        MappingFields = new List<KeyValuePair<string, object>> {
                            new KeyValuePair<string, object>("HOUSEID", "Id"),
                            new KeyValuePair<string, object>("HOUSEGUID", "HouseId"),
                            new KeyValuePair<string, object>("AOGUID", "AoId"),
                            new KeyValuePair<string, object>("null", "DivType"),
                            new KeyValuePair<string, object>("null", "HouseNum"),
                            new KeyValuePair<string, object>("null", "EstStatusId"),
                            new KeyValuePair<string, object>("null", "BuildNum"),
                            new KeyValuePair<string, object>("null", "StrucNum"),
                            new KeyValuePair<string, object>("null", "StrStatusId"),
                            new KeyValuePair<string, object>("null", "PostalCode"),
                            new KeyValuePair<string, object>("null", "OKATO"),
                            new KeyValuePair<string, object>("null", "OKTMO"),
                            new KeyValuePair<string, object>("null", "UpdateDate"),
                            new KeyValuePair<string, object>("null", "StartDate"),
                            new KeyValuePair<string, object>("null", "EndDate")
                            /*AOGUID	BUILDNUM	ENDDATE	ESTSTATUS	HOUSEGUID	HOUSEID	HOUSENUM	STATSTATUS
                         * IFNSFL	IFNSUL	OKATO	OKTMO	POSTALCODE	STARTDATE	STRUCNUM	STRSTATUS
                         * TERRIFNSFL	TERRIFNSUL	UPDATEDATE	NORMDOC	COUNTER	CADNUM	DIVTYPE*/
                        }
                    },
                    new FileTaskConversionDef {
                        FilePattern = "DADDROB.Xml",
                        SqlconversionStoreProc = "dbo.AsconversionDeleteAddrObjs",
                        Insert = false,
                        Update = false,
                        DeleteIfMatched = true,
                        DeleteIfNotMatchedBySource = false,
                        MappingFields = new List<KeyValuePair<string, object>> {
                            new KeyValuePair<string, object>("AOID", "Id"),
                            new KeyValuePair<string, object>("null", "PrevId"),
                            new KeyValuePair<string, object>("null", "NextId"),
                            new KeyValuePair<string, object>("AOGUID", "AoId"),
                            new KeyValuePair<string, object>("null", "ParentAoId"),
                            new KeyValuePair<string, object>("null", "DivType"),
                            new KeyValuePair<string, object>("null", "AoLevel"),
                            new KeyValuePair<string, object>("null", "TypeShortName"),
                            new KeyValuePair<string, object>("null", "Name"),
                            new KeyValuePair<string, object>("null", "OffName"),
                            new KeyValuePair<string, object>("null", "CentStatusId"),
                            new KeyValuePair<string, object>("null", "Code"),
                            new KeyValuePair<string, object>("null", "PlainCode"),
                            new KeyValuePair<string, object>("null", "PostalCode"),
                            new KeyValuePair<string, object>("null", "OKATO"),
                            new KeyValuePair<string, object>("null", "OKTMO"),
                            new KeyValuePair<string, object>("LIVESTATUS", "LiveStatusId"),
                            new KeyValuePair<string, object>("null", "ActStatusId"),
                            new KeyValuePair<string, object>("null", "OperStatusId"),
                            new KeyValuePair<string, object>("null", "UpdateDate"),
                            new KeyValuePair<string, object>("null", "StartDate"),
                            new KeyValuePair<string, object>("null", "EndDate")
                            /*ACTSTATUS	AOGUID	AOID	AOLEVEL	AREACODE	AUTOCODE	CENTSTATUS	CITYCODE	CODE
                         * CURRSTATUS	ENDDATE	FORMALNAME	IFNSFL	IFNSUL	NEXTID	OFFNAME	OKATO	OKTMO	OPERSTATUS
                         * PARENTGUID	PLACECODE	PLAINCODE	POSTALCODE	PREVID	REGIONCODE	SHORTNAME	STARTDATE
                         * STREETCODE	TERRIFNSFL	TERRIFNSUL	UPDATEDATE	CTARCODE	EXTRCODE	SEXTCODE
                         * LIVESTATUS	NORMDOC	PLANCODE	CADNUM	DIVTYPE*/
                        }
                    }
                }
            }
        };

        /// <summary>
        /// File conversion task.
        /// </summary>
        public class FileTaskconversion
        {
            private readonly ConversionContext _conversionContext;

            /// <summary>
            /// Initializes a new instance of the <see cref="FileTaskconversion"/> class.
            /// </summary>
            /// <param name="def">The definition.</param>
            /// <param name="conversionContext">The conversion context.</param>
            /// <exception cref="System.ArgumentNullException">
            /// def
            /// or
            /// conversionContext
            /// </exception>
            public FileTaskconversion([NotNull] FileTaskConversionDef def, [NotNull] ConversionContext conversionContext)
            {
                if (def == null)
                    throw new ArgumentNullException(nameof(def));
                if (conversionContext == null)
                    throw new ArgumentNullException(nameof(conversionContext));

                Def = def;
                _conversionContext = conversionContext;
            }

            /// <summary>
            /// Gets the definition.
            /// </summary>
            /// <value>
            /// The definition.
            /// </value>
            public FileTaskConversionDef Def { get; }

            /// <summary>
            /// Gets or sets the file.
            /// </summary>
            /// <value>
            /// The file.
            /// </value>
            public string File { get; set; }

            /// <summary>
            /// Runs the asynchronous.
            /// </summary>
            /// <param name="logStep">The parent log step.</param>
            /// <returns></returns>
            public async Task RunAsync(EventLogger logStep)
            {
                await Task.Factory.StartNew(
                    () =>
                    {
                        ExtractFileFromArchive(logStep);
                        var filePath = Path.Combine(_conversionContext.FiasXmlFolderPath, File);

                        // Перобразование файла Xml в файл CSV
                        XmlConversionThread(logStep, filePath);

                        DeleteFile(logStep, filePath);

                    });
            }

            private static void DeleteFile(EventLogger logStep, string filePath)
            {
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            private void XmlConversion(EventLogger logStep, string filePath)
            {
                var csvfileName = File.Replace(".Xml", ".csv", StringComparison.InvariantCultureIgnoreCase);
                logStep.Info($"Convert {File} to {csvfileName}");

                string XmlTableName = Path.GetFileNameWithoutExtension(File);

                var factory = new XmlConversionTasksFactory(_conversionContext, File);
                factory.CreateExecTask(XmlTableName);
            }

            private void XmlConversionThread(EventLogger logStep, string filePath)
            {
                // Необходимо запускать в отдельном потоке, так как Microsoft.ACE.OLEDB.12 не адекватен в рамках одного потока
                Exception error = null;
                var resetEvent = new ManualResetEvent(false);

                var thread = new Thread(
                    state => {
                        try
                        {
                            XmlConversion(logStep, filePath);
                        }
                        catch (Exception ex)
                        {
                            error = ex;
                            // throw; Ошибка в отдельном потоке не должна приводить к падению всего приложения
                        }
                        finally
                        {
                            resetEvent.Set();
                        }
                    });
                thread.Start();
                resetEvent.WaitOne();

                if (error != null)
                    throw new Exception($"Xmlconversion error: {error.Message}", error);
            }

            private void ExtractFileFromArchive(EventLogger logStep)
            {
                var sevenZipArchive = _conversionContext.Scope.Factory.Create7ZipArchive();
                sevenZipArchive.UnpackArchive(
                    _conversionContext.FiasXmlArchivePath,
                    _conversionContext.FiasXmlFolderPath,
                    Archive7Zip.OverwriteModes.s,
                    wildcardOrFileNameList: new[]
                    {
                        File
                    });
            }
        }

        /// <summary>
        /// Definition file conversion task.
        /// </summary>
        public class FileTaskConversionDef
        {
            /// <summary>
            /// Gets or sets the delete if matched.
            /// </summary>
            /// <value>
            /// The delete if matched.
            /// </value>
            public bool? DeleteIfMatched { get; set; }

            /// <summary>
            /// Gets or sets the delete if not matched by source.
            /// </summary>
            /// <value>
            /// The delete if not matched by source.
            /// </value>
            public bool? DeleteIfNotMatchedBySource { get; set; }

            /// <summary>
            /// Gets or sets the file pattern.
            /// </summary>
            /// <value>
            /// The file pattern.
            /// </value>
            public string FilePattern { get; set; }

            /// <summary>
            /// Gets or sets the insert.
            /// </summary>
            /// <value>
            /// The insert.
            /// </value>
            public bool? Insert { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is use region code.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance is use region code; otherwise, <c>false</c>.
            /// </value>
            public bool IsUseRegionCode { get; set; }

            /// <summary>
            /// Gets or sets the mapping fields.
            /// </summary>
            /// <value>
            /// The mapping fields.
            /// </value>
            public List<KeyValuePair<string, object>> MappingFields { get; set; }

            /// <summary>
            /// Gets or sets the SQL conversion store proc.
            /// </summary>
            /// <value>
            /// The SQL conversion store proc.
            /// </value>
            public string SqlconversionStoreProc { get; set; }

            /// <summary>
            /// Gets or sets the update.
            /// </summary>
            /// <value>
            /// The update.
            /// </value>
            public bool? Update { get; set; }

            /// <summary>
            /// Gets or sets the where is fias full Xml.
            /// </summary>
            /// <value>
            /// The where is fias full Xml.
            /// </value>
            public string WhereIsFiasFullXml { get; set; }

            /// <summary>
            /// Gets or sets the where filter.
            /// </summary>
            /// <value>
            /// The where filter.
            /// </value>
            public string WhereFilter { get; set; }
        }

        /// <summary>
        ///     Group tasks.
        /// </summary>
        public class GroupTaskConversion
        {
            private readonly ConversionContext _conversionContext;

            /// <summary>
            /// The tasks.
            /// </summary>
            public List<FileTaskconversion> Tasks = new List<FileTaskconversion>();

            /// <summary>
            /// Initializes a new instance of the <see cref="GroupTaskConversion"/> class.
            /// </summary>
            /// <param name="def">The definition.</param>
            /// <param name="conversionContext">The conversion context.</param>
            /// <exception cref="System.ArgumentNullException">
            /// def
            /// or
            /// conversionContext
            /// </exception>
            public GroupTaskConversion([NotNull] GroupTaskConversionDef def, [NotNull] ConversionContext conversionContext)
            {
                if (def == null)
                    throw new ArgumentNullException(nameof(def));
                if (conversionContext == null)
                    throw new ArgumentNullException(nameof(conversionContext));

                Def = def;
                _conversionContext = conversionContext;
            }

            /// <summary>
            /// Gets the definition.
            /// </summary>
            /// <value>
            /// The definition.
            /// </value>
            public GroupTaskConversionDef Def { get; }

            /// <summary>
            /// Gets or sets the type of the conversion.
            /// </summary>
            /// <value>
            /// The type of the conversion.
            /// </value>
            public GroupTaskConversionDef.ProcessingTypes ConversionType { get; set; }

            /// <summary>
            /// Runs the asynchronous.
            /// </summary>
            /// <returns></returns>
            /// <exception cref="System.Exception">Ошибки в задачах</exception>
            public async Task RunAsync(EventLogger logStep, AppSettings settings)
            {
                //using (var logStep = parentLogStep.CreateChild(
                //    $"{conversionType} group task {Tasks.Select(_ => _.File).Aggregate((s1, s2) => s1 + ", " + s2)}",
                //    new AddrSysGroupTaskStat()))
                try
                {
                    if (ConversionType == GroupTaskConversionDef.ProcessingTypes.Serial)
                    {
                        foreach (var fileTaskconversion in Tasks)
                        {
                            try
                            {
                                await fileTaskconversion.RunAsync(logStep);
                            }
                            catch (Exception ex)
                            {
                                logStep.Error(ex, ex.Message);
                                logStep.Error(ex, $"Task conversion file {fileTaskconversion.File} error: {ex.Message}");
                            }
                        }
                    }
                    else
                    {
                        var allTasks = new List<Task>();
                        try
                        {
                            var queue = new Queue<FileTaskconversion>(Tasks);

                            var tasks = new List<Task>();
                            //var flag = false;
                            while (true)
                            {
                                while (queue.Count > 0)
                                {
                                    if (tasks.Count == settings.AddrSysManagerParallelTaskCount)
                                        break;

                                    var fileTaskconversion = queue.Dequeue();
                                    var task = fileTaskconversion.RunAsync(logStep);
                                    tasks.Add(task);
                                    allTasks.Add(task);
                                }

                                if (tasks.Count == 0)
                                    break;

                                try
                                {
                                    await Task.WhenAny(tasks.ToArray());
                                }
                                catch (Exception ex)
                                {
                                    logStep.Error(ex, ex.Message);
                                    //flag = true;
                                    continue;
                                    //break;
                                    // throw;
                                }

                                tasks.RemoveAll(_ => _.IsCompleted || _.IsFaulted || _.IsCanceled);

                                //if (allTasks.Any(_ => _.IsFaulted))
                                //    break;
                            }

                            //if (flag || tasks.Count > 0)
                            if (tasks.Count > 0)
                            {
                                tasks.RemoveAll(_ => _.IsCompleted || _.IsFaulted || _.IsCanceled);
                                if (tasks.Count > 0)
                                    await Task.WhenAll(tasks.ToArray());
                            }
                        }
                        finally
                        {
                            var isError = false;
                            foreach (var task in allTasks)
                            {
                                if (task.IsFaulted)
                                {
                                    isError = true;
                                    logStep.Error(task.Exception, task.Exception.Message);
                                }
                            }

                            if (isError)
                                throw new Exception("Ошибки в задачах");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logStep.Error(ex, ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        ///     Definition group tasks.
        /// </summary>
        public class GroupTaskConversionDef
        {
            /// <summary>
            /// Run tasks modes.
            /// </summary>
            public enum ProcessingTypes
            {
                /// <summary>
                /// The consistently or serial
                /// </summary>
                Serial,

                /// <summary>
                /// The parallel.
                /// </summary>
                Parallel
            }

            /// <summary>
            /// The tasks definition.
            /// </summary>
            public List<FileTaskConversionDef> Tasks = new List<FileTaskConversionDef>();

            /// <summary>
            /// Gets or sets run task modes.
            /// </summary>
            /// <value>
            /// The type of the conversion.
            /// </value>
            public ProcessingTypes ConversionType { get; set; }
        }
    }
}