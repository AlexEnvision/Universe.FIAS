using System;
using System.Collections.Generic;
using Universe.Fias.Core.Infrastructure;
using Universe.Fias.DataContracts.Models;
using Universe.Fias.DataContracts.Stat;

namespace Universe.Fias.Import.Algorithms.AddressSystem.Import.CSV
{
    public class CsvRecordsImporter
    {
        private readonly UniverseFiasScope _scope;

        /// <summary>
        ///     Инициализирует экземпляр класса <see cref="CsvRecordsImporter"/>
        /// </summary>
        /// <param name="scope">Сфера деятельности</param>
        public CsvRecordsImporter(UniverseFiasScope scope)
        {
            _scope = scope;
        }

        public IEnumerable<ImportStatItem> Import<TAsRecord, TEntityDb>(CsvRecordsToDbEntitiesMapping<TAsRecord, TEntityDb> mapping, IList<TAsRecord> records)
            where TAsRecord : AsRecord, new()
            where TEntityDb : class, new()
        {
            if (mapping.ExecuteCommands == null)
                throw new ArgumentNullException(nameof(mapping.ExecuteCommands));

            var results = mapping.ExecuteCommands.Invoke(_scope, records);

            foreach (var baseCommandResult in results)
            {
                yield return new ImportStatItem {

                };
            }
        }
    }
}