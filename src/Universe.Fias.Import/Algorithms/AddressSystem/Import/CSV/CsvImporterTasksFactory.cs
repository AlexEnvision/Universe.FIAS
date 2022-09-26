using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Universe.Fias.DataAccess.Models;
using Universe.Fias.DataContracts.Consts;
using Universe.Fias.DataContracts.Models;
using Universe.Fias.DataContracts.Stat;
using Universe.Helpers.Extensions;

namespace Universe.Fias.Import.Algorithms.AddressSystem.Import.CSV
{
    public class CsvImporterTasksFactory
    {
        private ImportContext _importContext;
        private string _file;

        public CsvImporterTasksFactory(ImportContext importContext, string file)
        {
            _importContext = importContext;
            _file = file;
        }

        public List<ImportStatItem> CreateExecTask(string dbfTableName, AddrSysImportManager.FileTaskImportDef def)
        {
            if (dbfTableName.StartsWith(DbfFilePatterns.ADDROB))
                dbfTableName = DbfFilePatterns.ADDROB;

            if (dbfTableName.StartsWith(DbfFilePatterns.HOUSE))
                dbfTableName = DbfFilePatterns.HOUSE;

            if (dbfTableName.StartsWith(DbfFilePatterns.DHOUSE))
                dbfTableName = DbfFilePatterns.DHOUSE;

            if (dbfTableName.StartsWith(DbfFilePatterns.DADDROB))
                dbfTableName = DbfFilePatterns.DADDROB;

            switch (dbfTableName)
            {
                case DbfFilePatterns.ACTSTAT:
                    return ExecCsvImportTask<DbfMetaInfo.ACTSTAT, AsActStatus>(def);

                case DbfFilePatterns.CENTERST:
                    return ExecCsvImportTask<DbfMetaInfo.CENTERST, AsCenterStatus>(def);

                case DbfFilePatterns.ESTSTAT:
                    return ExecCsvImportTask<DbfMetaInfo.ESTSTAT, AsEstStatus>(def);

                case DbfFilePatterns.OPERSTAT:
                    return ExecCsvImportTask<DbfMetaInfo.OPERSTAT, AsOperStatus>(def);

                case DbfFilePatterns.SOCRBASE:
                    return ExecCsvImportTask<DbfMetaInfo.SOCRBASE, AsAddrObjType>(def);

                case DbfFilePatterns.STRSTAT:
                    return ExecCsvImportTask<DbfMetaInfo.STRSTAT, AsStrStatus>(def);

                case DbfFilePatterns.ADDROB:
                    return ExecCsvImportTask<DbfMetaInfo.ADDROB, AsAddrObj>(def);

                case DbfFilePatterns.HOUSE:
                    return ExecCsvImportTask<DbfMetaInfo.HOUSE, AsHouse>(def);

                // Файл используется для удаления. Т.к удаляется таблица AsHouses целиком, то обработка этих файлов пока не нужна
                //case DbfFilePatterns.DHOUSE:
                //    return ExecCsvImportTask<DbfMetaInfo.DHOUSE, AsHouse>(def);

                // Файл используется для удаления. Т.к удаляется таблица AsAddrObjs целиком, то обработка этих файлов пока не нужна
                //case DbfFilePatterns.DADDROB:
                //    return ExecCsvImportTask<DbfMetaInfo.DADDROB, AsAddrObj>(def);
            }

            return new List<ImportStatItem>();
        }

        private List<ImportStatItem> ExecCsvImportTask<TAsRecord, TEntityDb>(AddrSysImportManager.FileTaskImportDef def)
            where TAsRecord : AsRecord, new()
            where TEntityDb : class, new()
        {
            if (def.Mapping is CsvRecordsToDbEntitiesMapping<TAsRecord, TEntityDb> mapping)
            {
                mapping.RegionCode = def.IsUseRegionCode ? Regex.Match(_file, "\\d+").Value : null;

                if (_importContext.FiasCsvFolderPath.IsNullOrEmpty())
                    throw new ArgumentNullException(nameof(_importContext.FiasCsvFolderPath));
                
                var csvfileName = _file.Replace(".dbf", ".csv", StringComparison.InvariantCultureIgnoreCase);
                var reader = new CsvAsRecordsReader(_importContext.FiasCsvFolderPath);
                var records = reader.Read<TAsRecord>(csvfileName);

                var importer = new CsvRecordsImporter(_importContext.Scope);
                var results = importer.Import(mapping, records).ToList();
                return results;
            }

            return new List<ImportStatItem>();
        }
    }
}