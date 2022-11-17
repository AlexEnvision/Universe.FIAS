using System;
using Universe.Fias.DataAccess;
using Universe.Fias.IO.Compression;
using Universe.IO.Security.Principal;

namespace Universe.Fias.Core.Infrastructure
{
    /// <summary>
    ///     Universe Fias Factory.
    /// </summary>
    /// <seealso cref="System.IDisposable"/>
    public interface IUniverseFiasFactory : IDisposable
    {
        /// <summary>
        /// Create <see cref="Archive7Zip"/>.
        /// </summary>
        /// <returns><see cref="Archive7Zip"/>.</returns>
        Archive7Zip Create7ZipArchive();

        /// <summary>
        ///     Выполняет имперсонализацию к учетке пула приложения,
        ///     обязательно нужно вызвать Dispose.
        /// </summary>
        /// <returns></returns>
        RunAsAppPoolScope CreateRunAsAppPoolScope();

        /// <summary>
        ///     Создаёт контекст UniverseFiasDb, с конекшеном открытым заранее под учеткой пула,
        ///     обязательно нужно вызвать Dispose.
        /// </summary>
        /// <returns></returns>
        IUniverseFiasDbContext CreateUniverseFiasDb();
    }
}