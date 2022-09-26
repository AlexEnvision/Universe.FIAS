using System.Data.Entity.ModelConfiguration;
using Universe.DataAccess.Mappings.Framework;
using Universe.Fias.DataAccess.Mappings.Framework;
using Universe.Fias.DataAccess.Models;

namespace Universe.Fias.DataAccess.Mappings
{
    internal sealed class AsAddrObjDbTableMap : EntityMap<AsAddrObj>
    {
        protected override void Map(EntityTypeConfiguration<AsAddrObj> configuration)
        {
            configuration.ToTable("AsAddrObjs");

            configuration.HasOptional(x => x.ActStatus).WithMany(e => e.AsAddrObjs).HasForeignKey(x => x.ActStatusId).WillCascadeOnDelete(false);
            configuration.HasOptional(x => x.CenterStatus).WithMany(e => e.AsAddrObjs).HasForeignKey(x => x.CentStatusId).WillCascadeOnDelete(false);
            configuration.HasOptional(x => x.Type).WithMany(e => e.AddrObjs).HasForeignKey(x => x.TypeId).WillCascadeOnDelete(false);
            configuration.HasOptional(x => x.OperStatus).WithMany(e => e.AsAddrObjs).HasForeignKey(x => x.OperStatusId).WillCascadeOnDelete(false);
            configuration.HasOptional(x => x.OperStatus).WithMany(e => e.AsAddrObjs).HasForeignKey(x => x.OperStatusId).WillCascadeOnDelete(false);

            configuration.HasIndex(x => x.Name);
            configuration.HasIndex(x => x.Code);
            configuration.HasIndex(x => x.PlainCode);
            configuration.HasIndex(x => x.AoCode);
        }
    }
}