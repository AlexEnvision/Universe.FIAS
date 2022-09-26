using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Universe.CQRS.Dal.Commands.Base;
using Universe.CQRS.Models.Enums;
using Universe.Helpers.Extensions;
using Universe.SqlBulkTools;

namespace Universe.Fias.Import.Dal.Commands
{
    /// <summary>
    ///     Комманда пакетного обновления множества сущностей
    /// <author>Alex Envision</author>
    /// </summary>
    /// <typeparam name="TEntityDb"></typeparam>
    public class UpdateAsEntitiesBatchCommand<TEntityDb> : BaseCommand
        where TEntityDb : class
    {
        public string ConnectionString { get; set; }

        public virtual UpdateAsEntitiesBatchCommand<TEntityDb> Execute<TChildEntityDb>(
            Expression<Func<TEntityDb, object>> keySelector,
            TEntityDb entityDb,
            params IList<TChildEntityDb>[] entitiesDbs) where TChildEntityDb : class
        {
            BatchProcess(keySelector, new List<TEntityDb> { entityDb });
            return this;
        }

        public UpdateAsEntitiesBatchCommand<TEntityDb> BatchProcess<TChildEntityDb>(
             Expression<Func<TChildEntityDb, object>> keySelector,
             List<TChildEntityDb> entities)
             where TChildEntityDb : class
        {
            if (DbSystemManagementType == DbSystemManagementTypes.PostgreSQL)
                throw new NotSupportedException("Операция массового обновления элементов в таблицах на данный момент для PostgreSQL не поддерживается!");

            // Сохранение строки подключения
            ConnectionString = ConnectionString.IsNullOrEmpty() ? DbCtx.Database.Connection.ConnectionString : ConnectionString;

            var setDb = DbCtx.Set<TChildEntityDb>();

            var table = GetTableName(setDb);

            var bulk = new BulkOperations();

            //Expression<Func<TChildEntityDb, object>> keySelector =
            //    ExpressionExtensions.CreateExpressionDbeUniversal<TChildEntityDb>("Id");

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                bulk.Setup<TChildEntityDb>(x => x.ForCollection(entities))
                    .WithTable(table)
                    .AddAllColumns()
                    .BulkInsertOrUpdate()
                    .SetIdentityColumn(keySelector)
                    .MatchTargetOn(keySelector);

                bulk.CommitTransaction(connection);
            }

            return this;
        }

        public UpdateAsEntitiesBatchCommand<TEntityDb> BatchProcess<TParentEntity, TChildEntityDb>(
            Expression<Func<TChildEntityDb, object>> keySelector,
            List<TParentEntity> parentEntitiesAfterUpdate,
            Dictionary<TParentEntity, List<TChildEntityDb>> entitiesDict,
            Func<List<TChildEntityDb>, TParentEntity, List<TChildEntityDb>> parentKeySetterFunc)
            where TChildEntityDb : class
        {
            var setDb = DbCtx.Set<TChildEntityDb>();

            var table = GetTableName(setDb);

            var entitiesKvpList = entitiesDict.ToList();
            var entities = new List<TChildEntityDb>();
            for (var index = 0; index < entitiesDict.Count; index++)
            {
                var parentEntityDb = parentEntitiesAfterUpdate[index];
                var childEntityDbKvp = entitiesKvpList[index].Value;
                entities.AddRange(parentKeySetterFunc.Invoke(childEntityDbKvp, parentEntityDb) ?? new List<TChildEntityDb>());
            }

            // Чтение сохранённой ранее строки подключения т.к в случае повторного обращения к Database.Connection пароль теряется
            string connectionString = ConnectionString.IsNullOrEmpty() ? DbCtx.Database.Connection.ConnectionString : ConnectionString;
            var bulk = new BulkOperations();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                bulk.Setup<TChildEntityDb>(x => x.ForCollection(entities))
                    .WithTable(table)
                    .AddAllColumns()
                    .BulkInsertOrUpdate()
                    .SetIdentityColumn(keySelector)
                    .MatchTargetOn(keySelector);

                bulk.CommitTransaction(connection);
            }

            return this;
        }

        public string GetTableName<TEntityDb>(DbSet<TEntityDb> setDb, string schema = "dbo")
            where TEntityDb : class
        {
            //string sql = setDb.Sql;       //Для версии 6.2.0
            string sql = setDb.ToString();  //Для версии 6.1.3 Аналог Sql в 6.2.0

            Regex regex = new Regex(@"FROM\s+(?<table>.+)\s+AS");
            Match match = regex.Match(sql);

            string tableName = match.Groups["table"].Value;
            tableName = tableName.Replace($"[{schema}]", "").Replace(".", "").Replace("[", "").Replace("]", "");
            return tableName;
        }
    }
}
