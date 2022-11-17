
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using Universe.CQRS.Dal.Commands;
using Universe.CQRS.Dal.Commands.CommandResults.Base;
using Universe.CQRS.Dal.Queries;
using Universe.CQRS.Extensions;
using Universe.CQRS.Infrastructure;
using Universe.CQRS.Models.Enums;
using Universe.Diagnostic.Logger;
using Universe.Fias.Core.Infrastructure;
using Universe.Fias.DataAccess.Models;
using Universe.Fias.DataContracts.Consts;
using Universe.Fias.DataContracts.Models;
using Universe.Fias.Helpers;
using Universe.Fias.Import.Algorithms.AddressSystem.Import.CSV;
using Universe.Fias.Import.Dal.Commands;
using Universe.Fias.IO.Compression;
using Universe.Helpers.Extensions;
using Universe.Types.Collection;
using DateTimeExtensions = Universe.Fias.Helpers.DateTimeExtensions;

namespace Universe.Fias.Import.Algorithms.AddressSystem.Import
{
    public partial class AddrSysImportManager
    {
        /// <summary>
        /// The import tasks definition.
        /// </summary>
        internal readonly List<GroupTaskImportDef> ImportTasksDef = new List<GroupTaskImportDef> {
            new GroupTaskImportDef {
                ImportType = GroupTaskImportDef.ImportTypes.Serial,
                Tasks = new List<FileTaskImportDef> {
                    new FileTaskImportDef {
                        FilePattern = "ACTSTAT.CSV",
                        SqlImportStoreProc = "dbo.AsImportActStatuses",
                        Insert = true,
                        Update = true,
                        DeleteIfMatched = false,
                        DeleteIfNotMatchedBySource = false,
                        MappingFields = new List<KeyValuePair<string, object>> {
                            new KeyValuePair<string, object>("ACTSTATID", "Id"),
                            new KeyValuePair<string, object>("NAME", "Name"),
                            new KeyValuePair<string, object>("null", "ShortName")
                        },
                        Mapping = new CsvRecordsToDbEntitiesMapping<DbfMetaInfo.ACTSTAT, AsActStatus>
                        {
                            DbfTableName = DbfFilePatterns.ACTSTAT,
                            ExecuteCommands = (scope, records) =>
                            {
                                var entityDbs = (scope.GetQuery<GetEntitiesQuery<AsActStatus>>().Execute(
                                                        EntityReqHelper.GetAnyReq(MetaInfoHelper.FieldMap(
                                                            MetaInfoHelper.MapRule<AsActStatus>(nameof(AsActStatus.Id), x => x.Id)),
                                                            allItemsAsOnePage: true))
                                                    .Items ?? new List<AsActStatus>()).ToDictionary(x => x.Actstatid);
                                foreach (var record in records)
                                {
                                    if (!entityDbs.ContainsKey(record.ACTSTATID))
                                    {
                                        var entityDb = new AsActStatus
                                        {
                                            Name = record.NAME,
                                            Actstatid = record.ACTSTATID
                                        };

                                        entityDbs.Add(record.ACTSTATID, entityDb);
                                    }
                                }

                                var result = scope.GetCommand<UpdateEntitiesCommand<AsActStatus>>().Execute(entityDbs.Values.ToList());
                                return new MatList<BaseCommandResult> { result };
                            }
                        }
                    },
                    new FileTaskImportDef {
                        FilePattern = "CENTERST.CSV",
                        SqlImportStoreProc = "dbo.AsImportCenterStatuses",
                        Insert = true,
                        Update = true,
                        DeleteIfMatched = false,
                        DeleteIfNotMatchedBySource = false,
                        MappingFields = new List<KeyValuePair<string, object>> {
                            new KeyValuePair<string, object>("CENTERSTID", "Id"),
                            new KeyValuePair<string, object>("NAME", "Name"),
                            new KeyValuePair<string, object>("null", "ShortName")
                        },
                        Mapping = new CsvRecordsToDbEntitiesMapping<DbfMetaInfo.CENTERST, AsCenterStatus>
                        {
                            DbfTableName = DbfFilePatterns.CENTERST,
                            ExecuteCommands = (scope, records) =>
                            {
                                var entityDbs = (scope.GetQuery<GetEntitiesQuery<AsCenterStatus>>().Execute(
                                        EntityReqHelper.GetAnyReq(MetaInfoHelper.FieldMap(
                                            MetaInfoHelper.MapRule<AsCenterStatus>(nameof(AsCenterStatus.Id), x => x.Id)),
                                            allItemsAsOnePage: true))
                                    .Items ?? new List<AsCenterStatus>()).ToDictionary(x => x.Centerstid);
                                foreach (var record in records)
                                {
                                    if (!entityDbs.ContainsKey(record.CENTERSTID))
                                    {
                                        var entityDb = new AsCenterStatus
                                        {
                                            Name = record.NAME,
                                            Centerstid = record.CENTERSTID,
                                        };

                                        entityDbs.Add(record.CENTERSTID, entityDb);
                                    }
                                }

                                var result = scope.GetCommand<UpdateEntitiesCommand<AsCenterStatus>>().Execute(entityDbs.Values.ToList());
                                return new MatList<BaseCommandResult> { result };
                            }
                        }
                    },
                    new FileTaskImportDef {
                        FilePattern = "ESTSTAT.CSV",
                        SqlImportStoreProc = "dbo.AsImportEstStatuses",
                        Insert = true,
                        Update = true,
                        DeleteIfMatched = false,
                        DeleteIfNotMatchedBySource = false,
                        MappingFields = new List<KeyValuePair<string, object>> {
                            new KeyValuePair<string, object>("ESTSTATID", "Id"),
                            new KeyValuePair<string, object>("NAME", "Name"),
                            new KeyValuePair<string, object>("SHORTNAME", "ShortName")
                        },
                        Mapping = new CsvRecordsToDbEntitiesMapping<DbfMetaInfo.ESTSTAT, AsEstStatus>
                        {
                            DbfTableName = DbfFilePatterns.ESTSTAT,
                            ExecuteCommands = (scope, records) =>
                            {
                                var entityDbs = (scope.GetQuery<GetEntitiesQuery<AsEstStatus>>().Execute(
                                        EntityReqHelper.GetAnyReq(MetaInfoHelper.FieldMap(
                                            MetaInfoHelper.MapRule<AsEstStatus>(nameof(AsEstStatus.Id), x => x.Id)),
                                            allItemsAsOnePage: true))
                                    .Items ?? new List<AsEstStatus>()).ToDictionary(x => x.Eststatid);
                                foreach (var record in records)
                                {
                                    if (!entityDbs.ContainsKey(record.ESTSTATID))
                                    {
                                        var entityDb = new AsEstStatus
                                        {
                                            Name = record.NAME,
                                            ShortName = record.SHORTNAME,
                                            Eststatid = record.ESTSTATID,
                                        };

                                        entityDbs.Add(record.ESTSTATID, entityDb);
                                    }
                                }

                                var result = scope.GetCommand<UpdateEntitiesCommand<AsEstStatus>>().Execute(entityDbs.Values.ToList());
                                return new MatList<BaseCommandResult> { result };
                            }
                        }
                    },
                    new FileTaskImportDef {
                        FilePattern = "OPERSTAT.CSV",
                        SqlImportStoreProc = "dbo.AsImportOperStatuses",
                        Insert = true,
                        Update = true,
                        DeleteIfMatched = false,
                        DeleteIfNotMatchedBySource = false,
                        MappingFields = new List<KeyValuePair<string, object>> {
                            new KeyValuePair<string, object>("OPERSTATID", "Id"),
                            new KeyValuePair<string, object>("NAME", "Name"),
                            new KeyValuePair<string, object>("null", "ShortName")
                        },
                        Mapping = new CsvRecordsToDbEntitiesMapping<DbfMetaInfo.OPERSTAT, AsOperStatus>
                        {
                            DbfTableName = DbfFilePatterns.OPERSTAT,
                            ExecuteCommands = (scope, records) =>
                            {
                                var entityDbs = (scope.GetQuery<GetEntitiesQuery<AsOperStatus>>().Execute(
                                        EntityReqHelper.GetAnyReq(MetaInfoHelper.FieldMap(
                                            MetaInfoHelper.MapRule<AsOperStatus>(nameof(AsEstStatus.Id), x => x.Id)),
                                            allItemsAsOnePage: true))
                                    .Items ?? new List<AsOperStatus>()).ToDictionary(x => x.Operstatid);
                                foreach (var record in records)
                                {
                                    if (!entityDbs.ContainsKey(record.OPERSTATID))
                                    {
                                        var entityDb = new AsOperStatus
                                        {
                                            Name = record.NAME,
                                            Operstatid = record.OPERSTATID,
                                        };

                                        entityDbs.Add(record.OPERSTATID, entityDb);
                                    }
                                }

                                var result = scope.GetCommand<UpdateEntitiesCommand<AsOperStatus>>().Execute(entityDbs.Values.ToList());
                                return new MatList<BaseCommandResult> { result };
                            }
                        }
                    },
                    new FileTaskImportDef {
                        FilePattern = "SOCRBASE.CSV",
                        SqlImportStoreProc = "dbo.AsImportAddrObjTypes",
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
                        WhereFilter = "KOD_T_ST <> '0'",
                        Mapping = new CsvRecordsToDbEntitiesMapping<DbfMetaInfo.SOCRBASE, AsAddrObjType>
                        {
                            DbfTableName = DbfFilePatterns.SOCRBASE,
                            ExecuteCommands = (scope, records) =>
                            {
                                var entityDbs = (scope.GetQuery<GetEntitiesQuery<AsAddrObjType>>().Execute(
                                        EntityReqHelper.GetAnyReq(MetaInfoHelper.FieldMap(
                                            MetaInfoHelper.MapRule<AsAddrObjType>(nameof(AsAddrObjType.Id), x => x.Id)),
                                            allItemsAsOnePage: true))
                                    .Items ?? new List<AsAddrObjType>()).ToDictionary(x => x.Code);
                                foreach (var record in records)
                                {
                                    if (!entityDbs.ContainsKey(record.KOD_T_ST))
                                    {
                                        int.TryParse(record.LEVEL, out var level);

                                        var entityDb = new AsAddrObjType
                                        {
                                            Name = record.SOCRNAME,
                                            ShortName = record.SCNAME,
                                            Level = level,
                                            Code = record.KOD_T_ST
                                        };

                                        entityDbs.Add(record.KOD_T_ST, entityDb);
                                    }
                                }

                                var result = scope.GetCommand<UpdateEntitiesCommand<AsAddrObjType>>().Execute(entityDbs.Values.ToList());
                                return new MatList<BaseCommandResult> { result };
                            }
                        }
                    },
                    new FileTaskImportDef {
                        FilePattern = "STRSTAT.CSV",
                        SqlImportStoreProc = "dbo.AsImportStrStatuses",
                        Insert = true,
                        Update = true,
                        DeleteIfMatched = false,
                        DeleteIfNotMatchedBySource = false,
                        MappingFields = new List<KeyValuePair<string, object>> {
                            new KeyValuePair<string, object>("STRSTATID", "Id"),
                            new KeyValuePair<string, object>("NAME", "Name"),
                            new KeyValuePair<string, object>("SHORTNAME", "ShortName")
                        },
                        Mapping = new CsvRecordsToDbEntitiesMapping<DbfMetaInfo.STRSTAT, AsStrStatus>
                        {
                            DbfTableName = DbfFilePatterns.STRSTAT,
                            ExecuteCommands = (scope, records) =>
                            {
                                var entityDbs = (scope.GetQuery<GetEntitiesQuery<AsStrStatus>>().Execute(
                                        EntityReqHelper.GetAnyReq(MetaInfoHelper.FieldMap(
                                            MetaInfoHelper.MapRule<AsStrStatus>(nameof(AsStrStatus.Id), x => x.Id)),
                                            allItemsAsOnePage: true))
                                    .Items ?? new List<AsStrStatus>()).ToDictionary(x => x.Strstatid);
                                foreach (var record in records)
                                {
                                    if (!entityDbs.ContainsKey(record.STRSTATID))
                                    {
                                        var entityDb = new AsStrStatus
                                        {
                                            Name = record.NAME,
                                            ShortName = record.SHORTNAME,
                                            Strstatid = record.STRSTATID
                                        };

                                        entityDbs.Add(record.STRSTATID, entityDb);
                                    }
                                }

                                var result = scope.GetCommand<UpdateEntitiesCommand<AsStrStatus>>().Execute(entityDbs.Values.ToList());
                                return new MatList<BaseCommandResult> { result };
                            }
                        }
                    }
                }
            },
            new GroupTaskImportDef {
                ImportType = GroupTaskImportDef.ImportTypes.Serial,
                Tasks = new List<FileTaskImportDef> {
                    new FileTaskImportDef {
                        FilePattern = "ADDROB[0-9]{2}.CSV",
                        SqlImportStoreProc = "dbo.AsImportAddrObjs",
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
                        // WhereIsFiasFullDbf = "LIVESTATUS = 1" - грузим все
                        Mapping = new CsvRecordsToDbEntitiesMapping<DbfMetaInfo.ADDROB, AsAddrObj>
                        {
                            DbfTableName = DbfFilePatterns.ADDROB,
                            ExecuteCommands = (scope, records) =>
                            {
                                var centStatuses = scope.GetQuery<GetEntitiesQuery<AsCenterStatus>>().Execute(
                                        EntityReqHelper.GetAnyReq(MetaInfoHelper.FieldMap(
                                            MetaInfoHelper.MapRule<AsCenterStatus>(nameof(AsCenterStatus.Id), x => x.Id)),
                                            allItemsAsOnePage: true))
                                    .Items.ToDictionary(x => x.Centerstid.ToString());

                                var actStatuses = scope.GetQuery<GetEntitiesQuery<AsActStatus>>().Execute(
                                    EntityReqHelper.GetAnyReq(MetaInfoHelper.FieldMap(
                                        MetaInfoHelper.MapRule<AsActStatus>(nameof(AsActStatus.Id), x => x.Id)),
                                        allItemsAsOnePage: true))
                                    .Items.ToDictionary(x => x.Actstatid.ToString());

                                var operStatuses = scope.GetQuery<GetEntitiesQuery<AsOperStatus>>()
                                    .Execute(EntityReqHelper.GetAnyReq(MetaInfoHelper.FieldMap(
                                        MetaInfoHelper.MapRule<AsOperStatus>(nameof(AsOperStatus.Id), x => x.Id)),
                                        allItemsAsOnePage: true))
                                    .Items.ToDictionary(x => x.Operstatid.ToString());
                            
                                var addrObjTypes = scope.GetQuery<GetEntitiesQuery<AsAddrObjType>>()
                                    .Execute(EntityReqHelper.GetAnyReq(MetaInfoHelper.FieldMap(
                                            MetaInfoHelper.MapRule<AsAddrObjType>(nameof(AsAddrObjType.Id), x => x.Id)),
                                        allItemsAsOnePage: true))
                                    .Items.ToDictionary(x => x.Level + " " + x.ShortName);

                                var entityDbs = new List<AsAddrObj>();

                                for (var index = 0; index < records.Count; index++)
                                {
                                    var record = records[index];

                                    var divtype = IntExtensions.TryParseNullable(record.DIVTYPE);
                                    var level = IntExtensions.TryParseNullable(record.AOLEVEL);
                                    int.TryParse(record.LIVESTATUS, out var liveStatusId);

                                    centStatuses.TryGetValue(record.CENTSTATUS, out var centStatus);
                                    actStatuses.TryGetValue(record.ACTSTATUS, out var actStatus);
                                    operStatuses.TryGetValue(record.OPERSTATUS, out var operStatus);
                                    addrObjTypes.TryGetValue(record.AOLEVEL + " " + record.SHORTNAME,
                                        out var addrObjType);

                                    var updateDate = DateTimeExtensions.TryParseNullable(record.UPDATEDATE);
                                    var startDate = DateTimeExtensions.TryParseNullable(record.STARTDATE);
                                    var endDate = DateTimeExtensions.TryParseNullable(record.ENDDATE);

                                    // Построение AoCode
                                    string aoCode;
                                    if (!record.PLAINCODE.IsNullOrEmpty())
                                    {
                                        int endIndex = 0;
                                        switch (level)
                                        {
                                            case 1: endIndex = 2;
                                                break;
                                            case 3: endIndex = 5;
                                                break;
                                            case 4: endIndex = 8;
                                                break;
                                            case 6: endIndex = 11;
                                                break;
                                            case 65: endIndex = 15;
                                                break;
                                            case 7: endIndex = 15;
                                                break;

                                            default: endIndex = record.PLAINCODE.Length;
                                                break;
                                        }

                                        aoCode = record.PLAINCODE.Length >= endIndex - 1 
                                            ? record.PLAINCODE.Substring(0, endIndex) 
                                            : record.PLAINCODE;
                                    }
                                    else
                                    {
                                        aoCode = record.PLAINCODE;
                                    }

                                    // Обрезаем название адресного объекта до 128 символов
                                    var name = record.FORMALNAME.CutString(128);

                                    var entityDb = new AsAddrObj
                                    {
                                        AoId = !record.AOID.IsNullOrEmpty()
                                            ? new Guid(record.AOID)
                                            : throw new Exception(
                                                $"Ключевой уникальный идентификатор в строке {index + 1} не заполнен, либо имеет неверный формат."),
                                        PrevId = !record.PREVID.IsNullOrEmpty()
                                            ? new Guid(record.PREVID)
                                            : (Guid?) null,
                                        NextId = !record.NEXTID.IsNullOrEmpty()
                                            ? new Guid(record.NEXTID)
                                            : (Guid?) null,
                                        //AoGuid = new Guid(record.AOGUID),
                                        ParentAoId = !record.PARENTGUID.IsNullOrEmpty()
                                            ? new Guid(record.PARENTGUID)
                                            : (Guid?) null,
                                        DivType = divtype,
                                        AoLevel = level,
                                        TypeShortName = record.SHORTNAME,
                                        Name = name,
                                        OffName = record.OFFNAME,
                                        CentStatusId = centStatus?.Id,
                                        Code = record.CODE,
                                        PlainCode = record.PLAINCODE,
                                        PostalCode = record.POSTALCODE,
                                        OKATO = record.OKATO,
                                        OKTMO = record.OKTMO,
                                        LiveStatusId = liveStatusId,
                                        ActStatusId = actStatus?.Id,
                                        OperStatusId = operStatus?.Id,
                                        UpdateDate = updateDate,
                                        StartDate = startDate,
                                        EndDate = endDate,

                                        TypeId = addrObjType?.Id,
                                        TypeCode = addrObjType?.Code,
                                        AoCode = aoCode
                                    };
                                    entityDbs.Add(entityDb);
                                }

                                //TODO -> Очень костыльно. Надо исправить Batch-процессы для Universe
                                var command = scope.GetCommand<UpdateAsEntitiesBatchCommand<AsAddrObj>>();
                                command.ConnectionString = (scope as UniverseFiasScope)?.Settings.ConnectionString;

                                if (scope.DbSystemManagementType == DbSystemManagementTypes.MSSql)
                                {
                                    // Массовая вставка для MSSql
                                    var result = command.BatchProcess(keySelector: x => x.Id, entityDbs);
                                }

                                if (scope.DbSystemManagementType == DbSystemManagementTypes.PostgreSQL)
                                {
                                    // Обычная вставка для Postgres т.к BatchProcess не поддерживаются для Postgres
                                    var result = scope.GetCommand<AddEntitiesCommand<AsAddrObj>>().Execute(entityDbs);
                                }

                                return new MatList<BaseCommandResult> { };
                            }
                        },
                    }
                }
            },
            new GroupTaskImportDef {
                ImportType = GroupTaskImportDef.ImportTypes.Serial,
                Tasks = new List<FileTaskImportDef> {
                    new FileTaskImportDef {
                        FilePattern = "HOUSE[0-9]{2}.CSV",
                        SqlImportStoreProc = "dbo.AsImportHouses",
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
                        WhereFilter = "(AOGUID is not null and AOGUID <> '')",
                        // WhereIsFiasFullDbf = "DATE() between STARTDATE and ENDDATE"  - грузим все
                        Mapping = new CsvRecordsToDbEntitiesMapping<DbfMetaInfo.HOUSE, AsHouse>
                        {
                            DbfTableName = DbfFilePatterns.HOUSE,
                            ExecuteCommands = (scope, records) =>
                            {
                                var estStatuses = scope.GetQuery<GetEntitiesQuery<AsEstStatus>>().Execute(
                                    EntityReqHelper.GetAnyReq(MetaInfoHelper.FieldMap(
                                        MetaInfoHelper.MapRule<AsEstStatus>(nameof(AsEstStatus.Id), x => x.Id)),
                                        allItemsAsOnePage: true))
                                    .Items.ToDictionary(x => x.Eststatid.ToString());

                                var strStatuses = scope.GetQuery<GetEntitiesQuery<AsStrStatus>>()
                                    .Execute(EntityReqHelper.GetAnyReq(MetaInfoHelper.FieldMap(
                                        MetaInfoHelper.MapRule<AsStrStatus>(nameof(AsStrStatus.Id), x => x.Id)),
                                        allItemsAsOnePage: true))
                                    .Items.ToDictionary(x => x.Strstatid.ToString());

                                var entityDbs = new List<AsHouse>();

                                for (var index = 0; index < records.Count; index++)
                                {
                                    var record = records[index];

                                    var divtype = IntExtensions.TryParseNullable(record.DIVTYPE);

                                    estStatuses.TryGetValue(record.ESTSTATUS, out var estStatus);
                                    strStatuses.TryGetValue(record.STRSTATUS, out var strStatus);

                                    var updateDate = DateTimeExtensions.TryParseNullable(record.UPDATEDATE);
                                    var startDate = DateTimeExtensions.TryParseNullable(record.STARTDATE);
                                    var endDate = DateTimeExtensions.TryParseNullable(record.ENDDATE);

                                    var entityDb = new AsHouse
                                    {
                                        HouseId = !record.HOUSEID.IsNullOrEmpty()
                                            ? new Guid(record.HOUSEID)
                                            : throw new Exception(
                                                $"Уникальный идентификатор дома HOUSEID в строке {index + 1} не заполнен, либо имеет неверный формат."),
                                        //HouseGuid = !record.HOUSEGUID.IsNullOrEmpty()
                                        //    ? new Guid(record.HOUSEGUID)
                                        //    : throw new Exception(
                                        //        $"Уникальный идентификатор AOGUIDAOGUID в строке {index + 1} не заполнен, либо имеет неверный формат."),
                                        AoId = !record.AOGUID.IsNullOrEmpty()
                                            ? new Guid(record.AOGUID)
                                            : throw new Exception(
                                                $"Уникальный идентификатор AOGUID в строке {index + 1} не заполнен, либо имеет неверный формат."),
                                        DivType = divtype,
                                        HouseNum = record.HOUSENUM,
                                        EstStatusId = estStatus?.Id,
                                        BuildNum = record.BUILDNUM,
                                        StrucNum = record.STRUCNUM,
                                        StrStatusId = strStatus?.Id,
                                        PostalCode = record.POSTALCODE,
                                        OKATO = record.OKATO,
                                        OKTMO = record.OKTMO,
                                        UpdateDate = updateDate,
                                        StartDate = startDate,
                                        EndDate = endDate
                                    };
                                    entityDbs.Add(entityDb);
                                }

                                //TODO -> Очень костыльно. Надо исправить Batch-процессы для Universe
                                var command = scope.GetCommand<UpdateAsEntitiesBatchCommand<AsHouse>>();
                                command.ConnectionString = (scope as UniverseFiasScope)?.Settings.ConnectionString;

                                if (scope.DbSystemManagementType == DbSystemManagementTypes.MSSql)
                                {
                                    var result = command.BatchProcess(keySelector: x => x.Id, entityDbs);
                                }

                                if (scope.DbSystemManagementType == DbSystemManagementTypes.PostgreSQL)
                                {
                                    // Обычная вставка для Postgres т.к BatchProcess не поддерживаются для Postgres
                                    var result = scope.GetCommand<AddEntitiesCommand<AsHouse>>().Execute(entityDbs);
                                }

                                return new MatList<BaseCommandResult> { };
                            }
                        }
                    }
                }
            },
            new GroupTaskImportDef {
                ImportType = GroupTaskImportDef.ImportTypes.Serial,
                Tasks = new List<FileTaskImportDef> {
                    new FileTaskImportDef {
                        FilePattern = "DHOUSE.CSV",
                        SqlImportStoreProc = "dbo.AsImportDeleteHouses",
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
                        },
                        Mapping = new CsvRecordsToDbEntitiesMapping<DbfMetaInfo.DHOUSE, AsHouse>
                        {
                            DbfTableName = DbfFilePatterns.DHOUSE,

                            // Файл используется для удаления. Т.к удаляется таблица AsHouses целиком, то эти файлы пока не нужны
                            ExecuteCommands = (scope, records) => new MatList<BaseCommandResult> { }
                        }
                    },
                    new FileTaskImportDef {
                        FilePattern = "DADDROB.CSV",
                        SqlImportStoreProc = "dbo.AsImportDeleteAddrObjs",
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
                        },
                        Mapping = new CsvRecordsToDbEntitiesMapping<DbfMetaInfo.DHOUSE, AsHouse>
                        {
                            DbfTableName = DbfFilePatterns.DHOUSE,

                            // Файл используется для удаления. Т.к удаляется таблица AsAddrObjs целиком, то эти файлы пока не нужны
                            ExecuteCommands = (scope, records) => new MatList<BaseCommandResult> { }
                        }
                    }
                }
            }
        };

        /// <summary>
        /// File import task.
        /// </summary>
        public class FileTaskImport
        {
            private readonly ImportContext _importContext;

            /// <summary>
            /// Initializes a new instance of the <see cref="FileTaskImport"/> class.
            /// </summary>
            /// <param name="def">The definition.</param>
            /// <param name="importContext">The import context.</param>
            /// <exception cref="System.ArgumentNullException">
            /// def
            /// or
            /// importContext
            /// </exception>
            public FileTaskImport([NotNull] FileTaskImportDef def, [NotNull] ImportContext importContext)
            {
                if (def == null)
                    throw new ArgumentNullException(nameof(def));
                if (importContext == null)
                    throw new ArgumentNullException(nameof(importContext));

                Def = def;
                _importContext = importContext;
            }

            /// <summary>
            /// Gets the definition.
            /// </summary>
            /// <value>
            /// The definition.
            /// </value>
            public FileTaskImportDef Def { get; }

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
                //await Task.Factory.StartNew(
                //    () => {
                //        using (var logStep = parentLogStep.CreateChild(
                //            $"Processing {File}",
                //            new object()))
                //            logStep.Run(
                //                new {
                //                    logStep
                //                },
                //                action: ctx => {

                await Task.Factory.StartNew(
                    () => {
                            AsVersionFile asVersionFile = null;
                            try
                            {
                                // TODO -> Вернуть запись в БД информации о версии файлов и т.д
                                //_importContext.Scope.UniverseFiasDb(
                                //    db => {
                                //        asVersionFile = db.AsVersionFiles.FirstOrDefault(
                                //            _ => _.VersionId == _importContext.VersionId && _.FileName.ToLower() == File.ToLower());

                                //        if (asVersionFile == null)
                                //        {
                                //            asVersionFile = new AsVersionFile {
                                //                VersionId = _importContext.VersionId,
                                //                Created = DateTimeOffset.Now,
                                //                FileName = File
                                //            };
                                //            db.AsVersionFiles.Add(asVersionFile);
                                //            db.SaveChanges();
                                //        }
                                //    });

                                //Debug.Assert(asVersionFile != null, "asVersionFile != null");
                                //if (asVersionFile.IsProcessed ?? false)
                                //{
                                //    logStep.Info($"Skip file: {File} - loaded earlier");
                                //    //logStep.SetFinalStatus(LogStepStatusEnum.StepSkipped);
                                //    return;
                                //}

                                //_importContext.Scope.UniverseFiasDb(
                                //    db => {
                                //        db.AsVersionFiles.Attach(asVersionFile);
                                //        asVersionFile.IsProcessed = false;
                                //        asVersionFile.StartDateProcessing = DateTimeOffset.Now;
                                //        asVersionFile.EndDateProcessed = null;
                                //        db.SaveChanges();
                                //    });

                                //ExtractFileFromArchive(logStep);
                                var filePath = Path.Combine(_importContext.FiasCsvFolderPath, File);

                                // Импорт из файла CSV
                                CsvImportThread(logStep, filePath);

                                DeleteFile(logStep, filePath);

                                //_importContext.Scope.UniverseFiasDb(
                                //    db => {
                                //        db.AsVersionFiles.Attach(asVersionFile);
                                //        asVersionFile.IsProcessed = true;
                                //        asVersionFile.EndDateProcessed = DateTimeOffset.Now;
                                //        db.SaveChanges();
                                //    });
                            }
                            catch (Exception ex)
                            {
                                _importContext.Scope.UniverseFiasDb(
                                    db => {
                                        if (asVersionFile != null)
                                        {
                                            db.AsVersionFiles.Attach(asVersionFile);
                                            asVersionFile.IsProcessed = false;
                                            asVersionFile.EndDateProcessed = DateTimeOffset.Now;

                                            asVersionFile.Error =
                                                $@"Attempt: {asVersionFile.StartDateProcessing} - {asVersionFile
                                                    .EndDateProcessed}
Error: {ex.GetExceptionInfoMessageExceptionTypeStackTrace()}



" + (asVersionFile.Error ?? string.Empty);
                                        }

                                        db.SaveChanges();
                                    });

                                throw;
                            }
                    });
                    //});
            }

            private static void DeleteFile(EventLogger logStep, string filePath)
            {
                //using (var logStep = parentLogStep.CreateChild(
                //    $"Delete {filePath}",
                //    new object()))
                //    logStep.Run(
                //        new {
                //        },
                //        action: ctx => {
                            if (System.IO.File.Exists(filePath))
                                System.IO.File.Delete(filePath);
                //        });
            }

            private void CsvImport(EventLogger logStep, string filePath)
            {
                //using (var logStep = parentLogStep.CreateChild(
                //    $"Import from {filePath}",
                //    new AddrSysImportFileStat()))
                //    logStep.Run(
                //        new {
                //            logStep
                //        },
                //        action: ctx => {
                //_importContext.Scope.UniverseFiasDb(
                //    db =>
                //    {
                        logStep.Info($"Import {File}");

                        string dbfTableName = Path.GetFileNameWithoutExtension(File);
                        FileTaskImportDef def = Def;

                        var factory = new CsvImporterTasksFactory(_importContext, File);
                        factory.CreateExecTask(dbfTableName, def);

                        //using (var dbfImporter = new DbfImporter(_importContext.FiasDbfFolderPath, db))
                        //{
                        //var whereIsFiasFullDbf = _importContext.IsFiasFullDbf ? Def.WhereIsFiasFullDbf : null;
                        //var where = new[] { whereIsFiasFullDbf, Def.WhereFilter };

                        //var mapping = new DbfImportMapping
                        //{
                        //    DbfTableName = Path.GetFileNameWithoutExtension(File),
                        //    SqlImportStoreProc = Def.SqlImportStoreProc,
                        //    Insert = Def.Insert,
                        //    Update = Def.Update,
                        //    DeleteIfMatched = Def.DeleteIfMatched,
                        //    DeleteIfNotMatchedBySource = Def.DeleteIfNotMatchedBySource,
                        //    MappingFields = Def.MappingFields,
                        //    Where = string.Join(" and ", where.Where(s => !string.IsNullOrWhiteSpace(s))),
                        //    RegionCode = Def.IsUseRegionCode ? Regex.Match(File, "\\d+").Value : null
                        //};

                        //logStep.Info($"2 {File}");
                        ////ctx.logStep.Stat.CSVCountRecords = dbfImporter.CountRecords(mapping);
                        //logStep.Info($"3 {File}");
                        ////ctx.logStep.Stat.CSVCountRecordsForImport = dbfImporter.CountRecordsForImport(mapping);

                        ////ctx.logStep.UpdateStat();
                        //logStep.Info($"4 {File}");

                        //dbfImporter.Import(
                        //    mapping,
                        //    stat =>
                        //    {
                        //        //ctx.logStep.Stat.Stat = stat;
                        //        //ctx.logStep.UpdateStat();
                        //    },
                        //    _importContext.Scope.Settings.AddrSysImportManagerPortionRecCount);
                        //}
                //    }
                //);
                //});
            }

            private void CsvImportThread(EventLogger logStep, string filePath)
            {
                // Необходимо запускать в отдельном потоке, так как Microsoft.ACE.OLEDB.12 не адекватен в рамках одного потока
                Exception error = null;
                var resetEvent = new ManualResetEvent(false);

                var thread = new Thread(
                    state => {
                        try
                        {
                            CsvImport(logStep, filePath);
                        }
                        catch (Exception ex)
                        {
                            error = ex;
                            //parentLogStep.LogError(ex.Message, ex);
                            //parentLogStep.Error(ex);
                            // throw; Ошибка в отдельном потоке не должна приводить к падению всего приложения
                        }
                        finally
                        {
                            resetEvent.Set();
                        }
                    });
                thread.Start();
                //ThreadPool.QueueUserWorkItem(
                //    state => {
                //        try
                //        {
                //            DbfImport(parentLogStep, filePath);
                //        }
                //        catch (Exception ex)
                //        {
                //            error = ex;
                //            parentLogStep.LogError(ex.Message, ex);
                //            parentLogStep.Error(ex);
                //            // throw; Ошибка в отдельном потоке не должна приводить к падению всего приложения
                //        }
                //        finally
                //        {
                //            resetEvent.Set();
                //        }
                //    });

                resetEvent.WaitOne();

                if (error != null)
                    throw new Exception($"CsvImport error: {error.Message}", error);
            }

            private void ExtractFileFromArchive(EventLogger logStep)
            {
                //using (var logStep = parentLogStep.CreateChild(
                //    $"Extract {File} from archive",
                //    new object()))
                //    logStep.Run(
                //        new {
                //        },
                //        action: ctx => {
                            var sevenZipArchive = _importContext.Scope.Factory.Create7ZipArchive();
                            sevenZipArchive.UnpackArchive(
                                _importContext.FiasDbfArchivePath,
                                _importContext.FiasDbfFolderPath,
                                Archive7Zip.OverwriteModes.s,
                                wildcardOrFileNameList: new[] {
                                    File
                                });
                //});
            }
        }

        /// <summary>
        /// Defenition file import task.
        /// </summary>
        public class FileTaskImportDef
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
            /// Gets or sets the SQL import store proc.
            /// </summary>
            /// <value>
            /// The SQL import store proc.
            /// </value>
            public string SqlImportStoreProc { get; set; }

            /// <summary>
            /// Gets or sets the update.
            /// </summary>
            /// <value>
            /// The update.
            /// </value>
            public bool? Update { get; set; }

            /// <summary>
            /// Gets or sets the where is fias full DBF.
            /// </summary>
            /// <value>
            /// The where is fias full DBF.
            /// </value>
            public string WhereIsFiasFullDbf { get; set; }

            /// <summary>
            /// Gets or sets the where filter.
            /// </summary>
            /// <value>
            /// The where filter.
            /// </value>
            public string WhereFilter { get; set; }

            /// <summary>
            ///     Карта взаимосвязи записей DBF и сущностей БД
            /// </summary>
            public CsvRecordsToDbEntitiesMapping Mapping { get; set; }
        }

        /// <summary>
        /// Group tasks.
        /// </summary>
        public class GroupTaskImport
        {
            private readonly ImportContext _importContext;

            /// <summary>
            /// The tasks.
            /// </summary>
            public List<FileTaskImport> Tasks = new List<FileTaskImport>();

            /// <summary>
            /// Initializes a new instance of the <see cref="GroupTaskImport"/> class.
            /// </summary>
            /// <param name="def">The definition.</param>
            /// <param name="importContext">The import context.</param>
            /// <exception cref="System.ArgumentNullException">
            /// def
            /// or
            /// importContext
            /// </exception>
            public GroupTaskImport([NotNull] GroupTaskImportDef def, [NotNull] ImportContext importContext)
            {
                if (def == null)
                    throw new ArgumentNullException(nameof(def));
                if (importContext == null)
                    throw new ArgumentNullException(nameof(importContext));

                Def = def;
                _importContext = importContext;
            }

            /// <summary>
            /// Gets the definition.
            /// </summary>
            /// <value>
            /// The definition.
            /// </value>
            public GroupTaskImportDef Def { get; }

            /// <summary>
            /// Gets or sets the type of the import.
            /// </summary>
            /// <value>
            /// The type of the import.
            /// </value>
            public GroupTaskImportDef.ImportTypes ImportType { get; set; }

            /// <summary>
            /// Runs the asynchronous.
            /// </summary>
            /// <returns></returns>
            /// <exception cref="System.Exception">Ошибки в задачах</exception>
            public async Task RunAsync(EventLogger logStep, AppSettings settings)
            {
                //using (var logStep = parentLogStep.CreateChild(
                //    $"{ImportType} group task {Tasks.Select(_ => _.File).Aggregate((s1, s2) => s1 + ", " + s2)}",
                //    new AddrSysGroupTaskStat()))
                try
                {
                    if (ImportType == GroupTaskImportDef.ImportTypes.Serial)
                    {
                        foreach (var fileTaskImport in Tasks)
                        {
                            try
                            {
                                await fileTaskImport.RunAsync(logStep);
                            }
                            catch (Exception ex)
                            {
                                logStep.Error(ex, ex.Message);
                                logStep.Error(ex, $"Task import file {fileTaskImport.File} error: {ex.Message}");
                            }
                        }
                    }
                    else
                    {
                        var allTasks = new List<Task>();
                        try
                        {
                            var queue = new Queue<FileTaskImport>(Tasks);

                            var tasks = new List<Task>();
                            //var flag = false;
                            while (true)
                            {
                                while (queue.Count > 0)
                                {
                                    if (tasks.Count == settings.AddrSysManagerParallelTaskCount)
                                        break;

                                    var fileTaskImport = queue.Dequeue();
                                    var task = fileTaskImport.RunAsync(logStep);
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
        /// Defenition group tasks.
        /// </summary>
        public class GroupTaskImportDef
        {
            /// <summary>
            /// Run tasks modes.
            /// </summary>
            public enum ImportTypes
            {
                /// <summary>
                /// The consistently. Or Serial
                /// </summary>
                Serial,

                /// <summary>
                /// The parallel.
                /// </summary>
                Parallel
            }

            /// <summary>
            /// The tasks defenition.
            /// </summary>
            public List<FileTaskImportDef> Tasks = new List<FileTaskImportDef>();

            /// <summary>
            /// Gets or sets run task modes.
            /// </summary>
            /// <value>
            /// The type of the import.
            /// </value>
            public ImportTypes ImportType { get; set; }
        }
    }
}