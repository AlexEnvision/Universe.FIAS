using System.Data.Entity.ModelConfiguration;
using Universe.DataAccess.Mappings.Framework;
using Universe.Fias.DataAccess.Models;

namespace Universe.Fias.DataAccess.Mappings
{
    internal sealed class AsOperStatusDbTableMap : EntityMap<AsOperStatus>
    {
        protected override void Map(EntityTypeConfiguration<AsOperStatus> configuration)
        {
            configuration.ToTable("AsOperStatuses");

            configuration.HasMany(e => e.AsAddrObjs).WithOptional().HasForeignKey(c => c.OperStatusId).WillCascadeOnDelete(false);
        }
    }
}