using System;
using System.Collections.Generic;
using Universe.Fias.Core.Infrastructure.DatabaseContexts;
using Universe.Fias.DataAccess;
using Universe.Fias.IO.Compression;
using Universe.Helpers.Extensions;
using Universe.IO.Security.Principal;
using Universe.Types;

namespace Universe.Fias.Core.Infrastructure.Impl
{
    /// <summary>
    ///     Universe Fias factory.
    /// </summary>
    /// <seealso cref="DisposableObject"/>
    /// <seealso cref="IUniverseFiasFactory"/>
    public class UniverseFiasFactory : DisposableObject, IUniverseFiasFactory
    {
        private readonly List<IDisposable> _createdDisposableObjects = new List<IDisposable>();

        private readonly UniverseFiasScope _scope;

        private readonly object _sync = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="UniverseFiasFactory"/> class.
        /// </summary>
        /// <param name="scope">The CTX.</param>
        public UniverseFiasFactory(UniverseFiasScope scope)
        {
            _scope = scope;
        }

        /// <summary>
        /// Create <see cref="Archive7Zip"/>.
        /// </summary>
        /// <returns><see cref="Archive7Zip"/>.</returns>
        public Archive7Zip Create7ZipArchive() {
            lock (_sync)
                return _createdDisposableObjects.AddIfIDisposable(() => new Archive7Zip(_scope.Settings.SevenZipFolderPath));
        }

        /// <summary>
        ///     Выполняет имперсонализацию к учетке пула приложения,
        ///     обязательно нужно вызвать Dispose.
        /// </summary>
        /// <returns></returns>
        public RunAsAppPoolScope CreateRunAsAppPoolScope()
        {
            lock (_sync)
                return _createdDisposableObjects.AddIfIDisposable(() => new RunAsAppPoolScope());
        }

        /// <summary>
        /// Создает контекст UniverseFiasDb, с конекшеном открытым заранее под учеткой пула,
        /// обязательно нужно вызвать Dispose.
        /// </summary>
        /// <returns></returns>
        public IUniverseFiasDbContext CreateUniverseFiasDb()
        {
            lock (_sync)
            {
                var conStr = _scope.Settings.ConnectionString;
                using (new RunAsAppPoolScope())
                    return _createdDisposableObjects.AddIfIDisposable(() => UniverseFiasDbContextFactory.Initialize.Create(conStr, _scope.DbSystemManagementType));
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            lock (_sync)
                if (disposing)
                    _createdDisposableObjects.DisposeAndClear();
        }
    }
}