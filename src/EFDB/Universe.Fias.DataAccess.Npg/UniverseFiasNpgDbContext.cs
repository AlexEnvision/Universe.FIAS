using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Universe.DataAccess.Mappings.Framework;
using Universe.DataAccess.Npg;
using Universe.Fias.DataAccess.Extensions;
using Universe.Fias.DataAccess.Models;
using Universe.Fias.DataAccess.Npg.Migrations;

namespace Universe.Fias.DataAccess.Npg
{
    public class UniverseFiasNpgDbContext: UniverseNpgDbContext<UniverseFiasNpgDbContext, Configuration>, IUniverseFiasDbContext  
        // :DbContext - расскомментировать и заменить :UniverseNpgDbContext<UniverseFiasNpgDbContext, Configuration>, IUniverseFiasDbContext  при добавлении миграций
        // через Add-Migration
    {
        private SqlDbContextExt _ext;

        public UniverseFiasNpgDbContext()
            : base()
        {
            SetConfigurationOptions();
        }

        public UniverseFiasNpgDbContext(string connectionString) : base(connectionString)
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
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<UniverseFiasNpgDbContext, Configuration>(true, new Configuration
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
