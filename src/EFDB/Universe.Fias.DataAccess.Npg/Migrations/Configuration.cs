using System.Data.Entity.Migrations;
using Universe.DataAccess.TableContent.Config;

namespace Universe.Fias.DataAccess.Npg.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<UniverseFiasNpgDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(UniverseFiasNpgDbContext context)
        {
            //SeedingEntitiesConfig.Configure(context);
        }
    }
}