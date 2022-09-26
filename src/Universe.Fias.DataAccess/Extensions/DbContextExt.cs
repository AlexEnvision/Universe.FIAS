using System;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Universe.Types.Collection;

namespace Universe.Fias.DataAccess.Extensions
{
    /// <summary>
    /// Extension for <see cref="DbContext"/>.
    /// </summary>
    public class DbContextExt
    {
        /// <summary>
        /// The store by type <see cref="DbContext"/>.
        /// </summary>
        private static readonly StoreLockSlim<Type, DbCtxInfoCache> StoreByTypeDbCtx = new StoreLockSlim<Type, DbCtxInfoCache>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextExt"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <exception cref="System.ArgumentNullException">dbContext</exception>
        public DbContextExt(DbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            DbContext = dbContext;
        }

        /// <summary>
        /// Gets the database context.
        /// </summary>
        /// <value>
        /// The database context.
        /// </value>
        public DbContext DbContext { get; }

        /// <summary>
        /// Gets the object context.
        /// </summary>
        /// <value>
        /// The object context.
        /// </value>
        public ObjectContext ObjectContext => ((IObjectContextAdapter)DbContext).ObjectContext;

        /// <summary>
        /// Gets the database CTX information.
        /// </summary>
        /// <value>
        /// The database CTX information.
        /// </value>
        protected virtual DbCtxInfoCache DbCtxInfo => StoreByTypeDbCtx.GetOrCreate(DbContext.GetType(), () => new DbCtxInfoCache());

        /// <summary>
        /// Gets entity set name of type entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        public string GetEntitySetName<TEntity>()
        {
            return DbCtxInfo.GetEntitySetName<TEntity>(DbContext);
        }

        /// <summary>
        /// DbCtxInfoCache.
        /// </summary>
        protected sealed class DbCtxInfoCache : IDisposable
        {
            private readonly StoreLockSlim<Type, string> _storeEntitySetNamesByTypeEntity = new StoreLockSlim<Type, string>();

            /// <inheritdoc/>
            public void Dispose()
            {
                _storeEntitySetNamesByTypeEntity.Dispose();
            }

            /// <summary>
            /// Gets entity set name of type entity.
            /// </summary>
            /// <typeparam name="TEntity">The type of the entity.</typeparam>
            /// <param name="dbContext">The database context.</param>
            /// <returns></returns>
            public string GetEntitySetName<TEntity>(DbContext dbContext)
            {
                return _storeEntitySetNamesByTypeEntity.GetOrCreate(
                    typeof(TEntity),
                    () => {
                        var objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;
                        var entityContainer = objectContext.MetadataWorkspace.GetEntityContainer(
                            objectContext.DefaultContainerName,
                            DataSpace.CSpace);

                        var entityTypeName = typeof(TEntity).Name;
                        var entitySetName = entityContainer.BaseEntitySets
                            .Where(_ => _.ElementType.Name == entityTypeName)
                            .Select(_ => _.Name)
                            .First();
                        return entitySetName;
                    });
            }
        }
    }
}