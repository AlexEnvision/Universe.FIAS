using System.Data.Entity;

namespace Universe.Fias.DataAccess.Mappings.Framework
{
    public interface IUEntityMap
    {
        void Apply(DbModelBuilder builder);
    }
}