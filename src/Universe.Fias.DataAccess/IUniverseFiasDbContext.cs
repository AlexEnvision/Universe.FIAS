using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Universe.Fias.DataAccess.Models;

namespace Universe.Fias.DataAccess
{
    public interface IUniverseFiasDbContext : IDisposable
    {
        Database Database { get; }

        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        DbSet<AsActStatus> AsActStatuses { get; set; }

        DbSet<AsAddrObj> AsAddrObjs { get; set; }

        DbSet<AsCenterStatus> AsCenterStatuses { get; set; }

        DbSet<AsEstStatus> AsEstStatuses { get; set; }

        DbSet<AsHouse> AsHouses { get; set; }

        DbSet<AsOperStatus> AsOperStatuses { get; set; }

        DbSet<AsStrStatus> AsStrStatuses { get; set; }

        DbSet<AsVersion> AsVersions { get; set; }

        DbSet<AsAddrObjType> AsAddrObjTypes { get; set; }

        DbSet<AsVersionFile> AsVersionFiles { get; set; }

        /// <summary>
        ///     Миграция базы данных
        /// </summary>
        void Migrate(int commandTimeout = 300);

        /// <summary>
        ///     Сохранить изменения
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
    }
}