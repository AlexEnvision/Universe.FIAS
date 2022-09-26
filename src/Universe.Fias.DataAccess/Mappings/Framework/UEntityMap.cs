using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using Universe.DataAccess.Mappings.Framework;
using Universe.DataAccess.Models;
using Universe.Fias.DataAccess.Models.Base;

namespace Universe.Fias.DataAccess.Mappings.Framework
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public abstract class UEntityMap<T> : IUEntityMap
        where T : UEntity
    {
        public virtual void Apply(DbModelBuilder builder)
        {
            var configuration = builder.Entity<T>();
            configuration.Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            configuration.HasKey(x => x.Id);

            configuration.ToTable(typeof(T).Name, "dbo");
            Map(configuration);
        }

        protected abstract void Map(EntityTypeConfiguration<T> configuration);
    }
}