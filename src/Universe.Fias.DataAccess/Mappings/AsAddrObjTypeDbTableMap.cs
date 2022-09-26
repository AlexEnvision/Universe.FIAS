using System.Data.Entity.ModelConfiguration;
using Universe.DataAccess.Mappings.Framework;
using Universe.Fias.DataAccess.Models;

namespace Universe.Fias.DataAccess.Mappings
{
    internal sealed class AsAddrObjTypeDbTableMap : EntityMap<AsAddrObjType>
    {
        protected override void Map(EntityTypeConfiguration<AsAddrObjType> configuration)
        {
            configuration.ToTable("AsAddrObjTypes");

            configuration.HasMany(e => e.AddrObjs).WithOptional().HasForeignKey(c => c.TypeId).WillCascadeOnDelete(false);
        }
    }
}