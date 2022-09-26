using System.Collections.Generic;
using Universe.Fias.DataContracts.Consts;
using Universe.Fias.DataContracts.Models;
using Universe.Fias.DataContracts.Stat;

namespace Universe.Fias.DBF.Converter.x64.Algorithms.AddressSystem.Conversion.DBF
{
    public class DbfConversionTasksFactory
    {
        private ConversionContext _conversionContext;
        private string _file;

        public DbfConversionTasksFactory(ConversionContext conversionContext, string file)
        {
            _conversionContext = conversionContext;
            _file = file;
        }

        public List<ConversionStatItem> CreateExecTask(string dbfTableName)
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
                    return ExecDbfConversionTask<DbfMetaInfo.ACTSTAT>();

                case DbfFilePatterns.CENTERST:
                    return ExecDbfConversionTask<DbfMetaInfo.CENTERST>();

                case DbfFilePatterns.ESTSTAT:
                    return ExecDbfConversionTask<DbfMetaInfo.ESTSTAT>();

                case DbfFilePatterns.OPERSTAT:
                    return ExecDbfConversionTask<DbfMetaInfo.OPERSTAT>();

                case DbfFilePatterns.SOCRBASE:
                    return ExecDbfConversionTask<DbfMetaInfo.SOCRBASE>();

                case DbfFilePatterns.STRSTAT:
                    return ExecDbfConversionTask<DbfMetaInfo.STRSTAT>();

                case DbfFilePatterns.ADDROB:
                    return ExecDbfConversionTask<DbfMetaInfo.ADDROB>();

                case DbfFilePatterns.HOUSE:
                    return ExecDbfConversionTask<DbfMetaInfo.HOUSE>();

                case DbfFilePatterns.DHOUSE:
                    return ExecDbfConversionTask<DbfMetaInfo.DHOUSE>();

                case DbfFilePatterns.DADDROB:
                    return ExecDbfConversionTask<DbfMetaInfo.DADDROB>();
            }

            return new List<ConversionStatItem>();
        }

        private List<ConversionStatItem> ExecDbfConversionTask<TAsRecord>()
            where TAsRecord : AsRecord, new()
        {
            var converter = new DbfToAsRecordConverter(_conversionContext.FiasDbfFolderPath, _conversionContext.FiasCsvFolderPath);
            var records = converter.Convert<TAsRecord>(_file);

            return new List<ConversionStatItem>
            {
                new ConversionStatItem<TAsRecord>
                {
                    Count = records.Count,
                    Records = records
                }
            };
        }
    }
}