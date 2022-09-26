using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Universe.Fias.DataAccess.Models;
using Universe.DataAccess;
using Universe.DataAccess.Mappings.Framework;
using Universe.Fias.DataAccess.Extensions;
using Universe.Fias.DataAccess.Migrations;

namespace Universe.Fias.DataAccess
{
    public partial class UniverseFiasDbContext : UniverseDbContext
    {
        private SqlDbContextExt _ext;

        public UniverseFiasDbContext()
            : base("UniverseFiasDb")
        {
            SetConfigurationOptions();
        }

        public UniverseFiasDbContext(string connectionString) : base(connectionString)
        {
            SetConfigurationOptions();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // При использовании MigrateDatabaseToLatestVersion в веб-приложении запущенном в IIS выдаётся ошибка 52 в MSSQL.
            // Вот комментарий с https://entityframework.net/knowledge-base/32052398/sql-network-interface-error-52:
            // I am not sure how the exact relationship ist but I was able to resolve this issue.
            // I used the MigrateDatabaseToLatestVersion initializer
            // and apparently this was the problem I now changed it to CreateDatabaseIfNotExists initializer and it works fine.
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<UniverseFiasDbContext, Configuration>());

            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            var maps = typeof(UniverseFiasDbContext).Assembly
                .GetTypes()
                .Where(x => !x.IsAbstract && x.GetInterfaces().Contains(typeof(IEntityMap)))
                .Select(Activator.CreateInstance)
                .Cast<IEntityMap>();
            foreach (var map in maps)
            {
                map.Apply(modelBuilder);
            }
        }
    
        public virtual DbSet<AsActStatus> AsActStatuses { get; set; }

        public virtual DbSet<AsAddrObj> AsAddrObjs { get; set; }

        public virtual DbSet<AsCenterStatus> AsCenterStatuses { get; set; }

        public virtual DbSet<AsEstStatus> AsEstStatuses { get; set; }

        public virtual DbSet<AsHouse> AsHouses { get; set; }

        public virtual DbSet<AsOperStatus> AsOperStatuses { get; set; }

        public virtual DbSet<AsStrStatus> AsStrStatuses { get; set; }

        public virtual DbSet<AsVersion> AsVersions { get; set; }

        public virtual DbSet<AsAddrObjType> AsAddrObjTypes { get; set; }

        public virtual DbSet<AsVersionFile> AsVersionFiles { get; set; }

        public virtual int AsImportActStatuses(Nullable<bool> insert, Nullable<bool> update, Nullable<bool> deleteIfMatched, Nullable<bool> deleteIfNotMatchedBySource)
        {
            var insertParameter = insert.HasValue ?
                new ObjectParameter("insert", insert) :
                new ObjectParameter("insert", typeof(bool));
    
            var updateParameter = update.HasValue ?
                new ObjectParameter("update", update) :
                new ObjectParameter("update", typeof(bool));
    
            var deleteIfMatchedParameter = deleteIfMatched.HasValue ?
                new ObjectParameter("deleteIfMatched", deleteIfMatched) :
                new ObjectParameter("deleteIfMatched", typeof(bool));
    
            var deleteIfNotMatchedBySourceParameter = deleteIfNotMatchedBySource.HasValue ?
                new ObjectParameter("deleteIfNotMatchedBySource", deleteIfNotMatchedBySource) :
                new ObjectParameter("deleteIfNotMatchedBySource", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AsImportActStatuses", insertParameter, updateParameter, deleteIfMatchedParameter, deleteIfNotMatchedBySourceParameter);
        }
    
        public virtual int AsSetThereAreStreets()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AsSetThereAreStreets");
        }

        private void SetConfigurationOptions()
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        ///     Миграция базы данных
        /// </summary>
        public void Migrate(int commandTimeout = 300)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<UniverseFiasDbContext, Configuration>(true, new Configuration
            {
                CommandTimeout = commandTimeout
            }));
            Database.CommandTimeout = commandTimeout;
            Database.Initialize(true);
        }

        /// <summary>
        /// Gets the ext.
        /// </summary>
        /// <value>
        /// The ext.
        /// </value>
        public SqlDbContextExt Ext => _ext ?? (_ext = new SqlDbContextExt(this));
    }
}
