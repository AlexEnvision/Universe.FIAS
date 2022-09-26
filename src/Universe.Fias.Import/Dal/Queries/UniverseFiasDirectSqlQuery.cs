using System.Collections.Generic;
using Universe.CQRS.Dal.Base.Extensions;
using Universe.CQRS.Dal.Queries.Base;

namespace Universe.Fias.Import.Dal.Queries
{
    public class UniverseFiasDirectSqlQuery<TEntityDb> : BaseQuery where TEntityDb : class, new()
    {
        public RequestedPage<TEntityDb> Execute(string sqlQuery, int commandTimeout = 5 * 60)
        {
            DbCtx.Database.CommandTimeout = commandTimeout;
            List<TEntityDb> result = DbCtx.Database.SqlQuery<TEntityDb>(sqlQuery).ToListAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            return new RequestedPage<TEntityDb>()
            {
                Items = result,
                NextPageHavingItems = false
            };
        }
    }
}