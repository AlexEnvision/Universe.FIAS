using System;
using Unity;
using Universe.CQRS.Infrastructure;
using Universe.Fias.Core.Infrastructure.Impl;
using Universe.Fias.DataAccess;
using Universe.IO.Security.Principal;

namespace Universe.Fias.Core.Infrastructure
{
    /// <summary>
    ///     Cфера деятельности/Рамки/Возможности классификатора адресов Universe Fias с базой данных,
    ///     контекстом пользователя
    /// </summary>
    public class UniverseFiasScope : UniverseScope<UniverseFiasDbContext>
    {
        private readonly IWebAppPrincipalResolver _principalResolver;

        private IAppSettings _appSettings;

        /// <summary>
        ///     Инициализирует экземпляр класса <see cref="UniverseFiasScope"/>
        /// </summary>
        /// <param name="principalResolver">Разрешитель личности, выполнившей вход в веб-приложение</param>
        /// <param name="appSettings">Настройки веб-приложения</param>
        /// <param name="container">Unity-контейнер</param>
        public UniverseFiasScope(IWebAppPrincipalResolver principalResolver, IAppSettings appSettings, IUnityContainer container) : base(appSettings, container)
        {
            if (container == null) 
                throw new ArgumentNullException(nameof(container));

            _principalResolver = principalResolver ?? throw new ArgumentNullException(nameof(principalResolver));
            _appSettings = appSettings;

            Factory = new UniverseFiasFactory(this);
        }

        /// <summary>
        ///     Под учеткой пула, создает контекст UniverseFiasDbContext, и выполняет действие.
        /// </summary>
        /// <param name="action"></param>
        public void UniverseFiasDb(Action<UniverseFiasDbContext> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            using (new RunAsAppPoolScope())
            using (var db = new UniverseFiasDbContext(DbCtx.Database.Connection.ConnectionString))
                action(db);
        }

        /// <summary>
        ///     Gets the factory.
        /// </summary>
        /// <value>
        ///     The factory.
        /// </value>
        public IUniverseFiasFactory Factory { get; private set; }

        /// <summary>
        ///     Настройки
        /// </summary>
        public AppSettings Settings => _appSettings as AppSettings;
    }
}