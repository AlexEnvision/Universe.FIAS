using Universe.CQRS.Infrastructure;
using Universe.CQRS.Models.Enums;
using Universe.Fias.DataAccess;
using Universe.Fias.DataAccess.Npg;

namespace Universe.Fias.Core.Infrastructure.DatabaseContexts
{
    /// <summary>
    ///     Фабрика контекста БД
    /// </summary>
    public class UniverseFiasDbContextFactory
    {
        /// <summary>
        ///     Инициализирует фабрику
        /// </summary>
        public static UniverseFiasDbContextFactory Initialize => new UniverseFiasDbContextFactory();

        /// <summary>
        ///     Создаёт контекст БД в зависимостит от типа БД
        /// </summary>
        /// <param name="connectionString">Строка подключения к базе данных</param>
        /// <param name="dbSystemManagementType">Типа дазы данных</param>
        /// <returns></returns>
        public IUniverseFiasDbContext Create(
            string connectionString,
            DbSystemManagementTypes dbSystemManagementType)
        {
            switch (dbSystemManagementType)
            {
                case DbSystemManagementTypes.MSSql:
                    return new UniverseFiasDbContext(connectionString);

                case DbSystemManagementTypes.PostgreSQL: 
                    return new UniverseFiasNpgDbContext(connectionString);

                default:
                    return new UniverseFiasDbContext(connectionString);
            }
        }
    }
}