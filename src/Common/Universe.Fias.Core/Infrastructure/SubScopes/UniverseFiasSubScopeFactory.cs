using Unity;
using Universe.CQRS.Infrastructure;
using Universe.CQRS.Models.Enums;

namespace Universe.Fias.Core.Infrastructure.SubScopes
{
    internal class UniverseFiasSubScopeFactory
    {
        /// <summary>
        ///     Инициализирует фабрику
        /// </summary>
        public static UniverseFiasSubScopeFactory Initialize => new UniverseFiasSubScopeFactory();

        public IUniverseFiasSubScope Create(
            UniverseFiasScope mainscope,
            IWebAppPrincipalResolver principalResolver,
            IAppSettings appSettings,
            IUnityContainer container,
            DbSystemManagementTypes dbSystemManagementType)
        {
            switch (dbSystemManagementType)
            {
                case DbSystemManagementTypes.MSSql:
                    return new UniverseFiasSubScope(mainscope, principalResolver, appSettings, container);

                case DbSystemManagementTypes.PostgreSQL: 
                    return new UniverseFiasNpgSubScope(mainscope, principalResolver, appSettings, container);

                default:
                    return new UniverseFiasSubScope(mainscope, principalResolver, appSettings, container);
            }
        }
    }
}