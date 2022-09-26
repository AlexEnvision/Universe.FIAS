using System.Data.Entity.ModelConfiguration;
using Universe.DataAccess.Mappings.Framework;
using Universe.Fias.DataAccess.Models;

namespace Universe.Fias.DataAccess.Mappings
{
    internal sealed class AsVersionDbTableMap : EntityMap<AsVersion>
    {
        protected override void Map(EntityTypeConfiguration<AsVersion> configuration)
        {
            configuration.ToTable("AsVersions");

            //configuration.HasMany(e => e.AsVersionFiles).WithOptional().HasForeignKey(c => c.VersionId).WillCascadeOnDelete(false);
        }
    }
}