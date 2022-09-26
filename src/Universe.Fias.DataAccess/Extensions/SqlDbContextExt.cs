using System.Data.Entity;
using System.Data.SqlClient;

namespace Universe.Fias.DataAccess.Extensions
{
    /// <summary>
    /// Extension <see cref="DbContextExt"/> for MSSQL.
    /// </summary>
    public class SqlDbContextExt : DbContextExt<SqlConnection, SqlCommand, SqlParameterCollection, SqlParameter, SqlDataReader>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDbContextExt"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public SqlDbContextExt(DbContext dbContext) : base(dbContext)
        {
        }
    }
}