using System.Data.Entity.ModelConfiguration;
using Universe.DataAccess.Mappings.Framework;
using Universe.Fias.DataAccess.Models;

namespace Universe.Fias.DataAccess.Mappings
{
    internal sealed class AsActStatusDbTableMap : EntityMap<AsActStatus>
    {
        protected override void Map(EntityTypeConfiguration<AsActStatus> configuration)
        {
            configuration.ToTable("AsActStatuses");

            configuration.HasMany(e => e.AsAddrObjs).WithOptional().HasForeignKey(c => c.ActStatusId).WillCascadeOnDelete(false);
        }
    }
}