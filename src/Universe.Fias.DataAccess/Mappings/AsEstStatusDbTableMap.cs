using System.Data.Entity.ModelConfiguration;
using Universe.DataAccess.Mappings.Framework;
using Universe.Fias.DataAccess.Models;

namespace Universe.Fias.DataAccess.Mappings
{
    internal sealed class AsEstStatusDbTableMap : EntityMap<AsEstStatus>
    {
        protected override void Map(EntityTypeConfiguration<AsEstStatus> configuration)
        {
            configuration.ToTable("AsEstStatuses");

            configuration.HasMany(e => e.AsHouses).WithOptional().HasForeignKey(c => c.EstStatusId).WillCascadeOnDelete(false);
        }
    }
}