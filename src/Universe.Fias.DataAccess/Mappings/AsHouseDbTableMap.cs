using System.Data.Entity.ModelConfiguration;
using Universe.DataAccess.Mappings.Framework;
using Universe.Fias.DataAccess.Mappings.Framework;
using Universe.Fias.DataAccess.Models;

namespace Universe.Fias.DataAccess.Mappings
{
    internal sealed class AsHouseDbTableMap : EntityMap<AsHouse>
    {
        protected override void Map(EntityTypeConfiguration<AsHouse> configuration)
        {
            configuration.ToTable("AsHouses");

            configuration.HasOptional(x => x.EstStatus).WithMany(e => e.AsHouses).HasForeignKey(x => x.EstStatusId).WillCascadeOnDelete(false);
            configuration.HasOptional(x => x.StrStatus).WithMany(e => e.AsHouses).HasForeignKey(x => x.StrStatusId).WillCascadeOnDelete(false);

            configuration.HasIndex(x => x.AoId);
            configuration.HasIndex(x => x.HouseNum);
        }
    }
}