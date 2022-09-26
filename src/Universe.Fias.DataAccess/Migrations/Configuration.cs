using System.Data.Entity.Migrations;
using Universe.DataAccess.TableContent.Config;

namespace Universe.Fias.DataAccess.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<UniverseFiasDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            //AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(UniverseFiasDbContext context)
        {
            SeedingEntitiesConfig.Configure(context);
        }
    }
}