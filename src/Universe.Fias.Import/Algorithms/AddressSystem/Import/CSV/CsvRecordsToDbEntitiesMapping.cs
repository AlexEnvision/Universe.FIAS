using System;
using System.Collections.Generic;
using Universe.CQRS.Dal.Commands.CommandResults.Base;
using Universe.CQRS.Infrastructure;
using Universe.Fias.DataContracts.Models;
using Universe.Types.Collection;

namespace Universe.Fias.Import.Algorithms.AddressSystem.Import.CSV
{
    public class CsvRecordsToDbEntitiesMapping
    {
        /// <summary>
        /// Gets or sets the name of the DBF table.
        /// </summary>
        /// <value>
        /// The name of the DBF table.
        /// </value>
        public string DbfTableName { get; set; }

        /// <summary>
        /// Gets or sets the region code.
        /// </summary>
        /// <value>
        /// The region code.
        /// </value>
        public string RegionCode { get; set; }

        public CsvRecordsToDbEntitiesMapping()
        {
        }
    }

    public class CsvRecordsToDbEntitiesMapping<TAsRecord, TEntity> : CsvRecordsToDbEntitiesMapping
        where TAsRecord : AsRecord, new()
        where TEntity : class, new()
    {
        /// <summary>
        ///     Комманды создания либо обновления
        /// </summary>
        public Func<IUniverseScope, IList<TAsRecord>, MatList<BaseCommandResult>> ExecuteCommands { get; set; }

        public CsvRecordsToDbEntitiesMapping()
        {
            ExecuteCommands = (scope, record) => new MatList<BaseCommandResult>();
        }
    }
}