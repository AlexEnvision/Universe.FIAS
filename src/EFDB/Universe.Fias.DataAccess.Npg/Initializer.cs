using System.Data.Entity;

namespace Universe.Fias.DataAccess.Npg
{
    /// <summary>
    /// 
    /// </summary>
    public class Initializer : IDatabaseInitializer<UniverseFiasNpgDbContext>
    {
        /// <summary>
        /// Executes the strategy to initialize the database for the given context.
        /// </summary>
        /// <param name="context"> The context. </param>
        public void InitializeDatabase(UniverseFiasNpgDbContext context)
        {
        }
    }
}