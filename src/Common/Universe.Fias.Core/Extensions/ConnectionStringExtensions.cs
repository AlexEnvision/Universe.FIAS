using Universe.CQRS.Models.Enums;
using Universe.Helpers.Extensions;

namespace Universe.Fias.Core.Extensions
{
    /// <summary>
    ///     Расширения строки подключений
    /// </summary>
    public static class ConnectionStringExtensions
    {
        /// <summary>
        ///     Определение типа базы по строке подключения
        /// </summary>
        /// <param name="connectionString">
        ///     Строка подключения
        /// </param>
        /// <returns>
        ///     Возвращает тип базы данных из перечисления <see cref="DbSystemManagementTypes"/>.
        ///     По-умолчанию возвращает DbSystemManagementTypes.MSSql
        /// </returns>
        public static DbSystemManagementTypes DeterminateDbTypeFromConnectionString(this string connectionString)
        {
            var dbType = DbSystemManagementTypes.MSSql;
            if (connectionString.PrepareToCompare().StartsWith("data source"))
            {
                dbType = DbSystemManagementTypes.MSSql;
            }

            if (connectionString.PrepareToCompare().StartsWith("server"))
            {
                dbType = DbSystemManagementTypes.PostgreSQL;
            }

            return dbType;
        }
    }
}