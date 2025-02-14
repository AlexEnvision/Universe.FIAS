﻿using System;
using System.Data.Entity;
using Unity;
using Universe.CQRS.Infrastructure;
using Universe.CQRS.Models.Dto;
using Universe.CQRS.Models.Enums;
using Universe.Fias.Core.Infrastructure.DatabaseContexts;
using Universe.Fias.Core.Infrastructure.SubScopes;
using Universe.Fias.DataAccess;
using Universe.IO.Security.Principal;

namespace Universe.Fias.Core.Infrastructure
{
    /// <summary>
    ///     Cфера деятельности/Рамки/Возможности классификатора адресов Universe Fias с базой данных,
    ///     контекстом пользователя
    /// </summary>
    public class UniverseFiasScope : IUniverseFiasSubScope
    {
        private readonly IWebAppPrincipalResolver _principalResolver;

        private IAppSettings _appSettings;

        private IUniverseFiasSubScope _subScope;

        private DbSystemManagementTypes _dbSystemManagementType;

        /// <summary>
        ///     Инициализирует экземпляр класса <see cref="UniverseFiasScope"/>
        /// </summary>
        /// <param name="dbSystemManagementType">Тип базы данных</param>
        /// <param name="principalResolver">Разрешитель личности, выполнившей вход в веб-приложение</param>
        /// <param name="appSettings">Настройки веб-приложения</param>
        /// <param name="container">Unity-контейнер</param>
        public UniverseFiasScope(IWebAppPrincipalResolver principalResolver, IAppSettings appSettings, IUnityContainer container, DbSystemManagementTypes dbSystemManagementType = DbSystemManagementTypes.MSSql) //: base(appSettings, container)
        {
            if (container == null) 
                throw new ArgumentNullException(nameof(container));

            _principalResolver = principalResolver ?? throw new ArgumentNullException(nameof(principalResolver));
            _appSettings = appSettings;
            _dbSystemManagementType = dbSystemManagementType;

            _subScope = UniverseFiasSubScopeFactory.Initialize.Create(this, principalResolver, appSettings, container, dbSystemManagementType);

            Factory = _subScope.Factory;
        }

        /// <summary>
        ///     Под учеткой пула, создает контекст UniverseFiasDbContext, и выполняет действие.
        /// </summary>
        /// <param name="action"></param>
        public void UniverseFiasDb(Action<IUniverseFiasDbContext> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            using (new RunAsAppPoolScope())
            using (var db = UniverseFiasDbContextFactory.Initialize.Create(DbCtx.Database.Connection.ConnectionString, _dbSystemManagementType))
                action(db);
        }

        /// <summary>
        ///     Получает фабрику
        ///     Gets the factory.
        /// </summary>
        /// <value>
        ///     The factory.
        /// </value>
        public IUniverseFiasFactory Factory { get; }

        /// <summary>
        ///     Настройки
        /// </summary>
        public AppSettings Settings => _appSettings as AppSettings;

        public DbSystemManagementTypes DbSystemManagementType
        {
            get => _subScope.DbSystemManagementType;
            set => _subScope.DbSystemManagementType = value;
        }

        public IUnityContainer Container => _subScope.Container;

        public DbContext DbCtx => _subScope.DbCtx;

        public UnitOfWork UnitOfWork => _subScope.UnitOfWork;

        public UserDto CurrentUser => _subScope.CurrentUser;
    }
}