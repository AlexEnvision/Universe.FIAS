using System;
using System.Collections.Generic;
using System.IO;
using Universe.Fias.DataContracts.Models;
using Universe.Helpers.Extensions;
using Universe.IO.CsvSerialization;

namespace Universe.Fias.Import.Algorithms.AddressSystem.Import.CSV
{
    public class CsvAsRecordsReader
    {
        private string _csvFolderPath;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CsvAsRecordsReader"/> class.
        /// </summary>
        /// <param name="csvFolderPath">The CSV folder path.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public CsvAsRecordsReader(string csvFolderPath)
        {
            if (csvFolderPath.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(csvFolderPath));

            _csvFolderPath = csvFolderPath;
        }

        public IList<T> Read<T>(string csvfileName)
            where T : AsRecord, new()
        {
            if (csvfileName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(csvfileName));

            IList<T> records;
            using (var fs = System.IO.File.Open(csvfileName, FileMode.Open))
            {
                var serializer = new CsvSerializer<T>
                {
                    Separator = '|',
                    UseTextQualifier = true,
                    IgnoreReferenceTypesExceptString = true
                };

                records = serializer.Deserialize(fs);
            }

            return records;
        }
    }
}