using System;
using Universe.CQRS.Infrastructure;
using Universe.Fias.DataAccess;

namespace Universe.Fias.Core.Infrastructure.SubScopes
{
    internal interface IUniverseFiasSubScope : IUniverseScope
    {
        /// <summary>
        ///     Под учеткой пула, создает контекст UniverseFiasDbContext, и выполняет действие.
        /// </summary>
        /// <param name="action"></param>
        void UniverseFiasDb(Action<IUniverseFiasDbContext> action);

        /// <summary>
        ///     Получает фабрику
        ///     Gets the factory.
        /// </summary>
        /// <value>
        ///     Фабрика.
        ///     The factory.
        /// </value>
        IUniverseFiasFactory Factory { get;  }

        /// <summary>
        ///     Настройки
        /// </summary>
        AppSettings Settings { get; }
    }
}