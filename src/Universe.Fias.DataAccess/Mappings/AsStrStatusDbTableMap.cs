using System.Data.Entity.ModelConfiguration;
using Universe.DataAccess.Mappings.Framework;
using Universe.Fias.DataAccess.Models;

namespace Universe.Fias.DataAccess.Mappings
{
    internal sealed class AsStrStatusDbTableMap : EntityMap<AsStrStatus>
    {
        protected override void Map(EntityTypeConfiguration<AsStrStatus> configuration)
        {
            configuration.ToTable("AsStrStatuses");

            configuration.HasMany(e => e.AsHouses).WithOptional().HasForeignKey(c => c.StrStatusId).WillCascadeOnDelete(false);
        }
    }
}