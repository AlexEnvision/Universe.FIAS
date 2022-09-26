using System.Data.Entity.ModelConfiguration;
using Universe.DataAccess.Mappings.Framework;
using Universe.Fias.DataAccess.Models;

namespace Universe.Fias.DataAccess.Mappings
{
    internal sealed class AsCenterStatusDbTableMap : EntityMap<AsCenterStatus>
    {
        protected override void Map(EntityTypeConfiguration<AsCenterStatus> configuration)
        {
            configuration.ToTable("AsCenterStatuses");

            configuration.HasMany(e => e.AsAddrObjs).WithOptional().HasForeignKey(c => c.CentStatusId).WillCascadeOnDelete(false);
        }
    }
}