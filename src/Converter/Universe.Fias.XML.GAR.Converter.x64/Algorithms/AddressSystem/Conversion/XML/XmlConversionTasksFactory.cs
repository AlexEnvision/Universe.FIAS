using System.Collections.Generic;
using Universe.Fias.DataContracts.Consts;
using Universe.Fias.DataContracts.Models;
using Universe.Fias.DataContracts.Stat;

namespace Universe.Fias.XML.GAR.Converter.x64.Algorithms.AddressSystem.Conversion.XML
{
    public class XmlConversionTasksFactory
    {
        private ConversionContext _conversionContext;
        private string _file;

        public XmlConversionTasksFactory(ConversionContext conversionContext, string file)
        {
            _conversionContext = conversionContext;
            _file = file;
        }

        public List<ConversionStatItem> CreateExecTask(string xmlTableName)
        {
            if (xmlTableName.StartsWith(XmlFilePatterns.ADDROB))
                xmlTableName = XmlFilePatterns.ADDROB;

            if (xmlTableName.StartsWith(XmlFilePatterns.HOUSE))
                xmlTableName = XmlFilePatterns.HOUSE;

            if (xmlTableName.StartsWith(XmlFilePatterns.DHOUSE))
                xmlTableName = XmlFilePatterns.DHOUSE;

            if (xmlTableName.StartsWith(XmlFilePatterns.DADDROB))
                xmlTableName = XmlFilePatterns.DADDROB;

            switch (xmlTableName)
            {
                case XmlFilePatterns.ACTSTAT:
                    return ExecXmlConversionTask<XmlGarMetaInfo.ACTSTAT>();

                case XmlFilePatterns.CENTERST:
                    return ExecXmlConversionTask<XmlGarMetaInfo.CENTERST>();

                case XmlFilePatterns.ESTSTAT:
                    return ExecXmlConversionTask<XmlGarMetaInfo.ESTSTAT>();

                case XmlFilePatterns.OPERSTAT:
                    return ExecXmlConversionTask<XmlGarMetaInfo.OPERSTAT>();

                case XmlFilePatterns.SOCRBASE:
                    return ExecXmlConversionTask<XmlGarMetaInfo.SOCRBASE>();

                case XmlFilePatterns.STRSTAT:
                    return ExecXmlConversionTask<XmlGarMetaInfo.STRSTAT>();

                case XmlFilePatterns.ADDROB:
                    return ExecXmlConversionTask<XmlGarMetaInfo.ADDROB>();

                case XmlFilePatterns.HOUSE:
                    return ExecXmlConversionTask<XmlGarMetaInfo.HOUSE>();

                case XmlFilePatterns.DHOUSE:
                    return ExecXmlConversionTask<XmlGarMetaInfo.DHOUSE>();

                case XmlFilePatterns.DADDROB:
                    return ExecXmlConversionTask<XmlGarMetaInfo.DADDROB>();
            }

            return new List<ConversionStatItem>();
        }

        private List<ConversionStatItem> ExecXmlConversionTask<TAsRecord>()
            where TAsRecord : AsRecord, new()
        {
            var converter = new XmlToAsRecordConverter(_conversionContext.FiasXmlFolderPath, _conversionContext.FiasCsvFolderPath);
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