using System;
using System.Threading;
using System.Threading.Tasks;

namespace Universe.Fias.Helpers
{
    /// <summary>
    ///     Вспомогательный класс для работы с асинхронными методами
    /// </summary>
    public static class AsyncHelper
    {
        private static readonly TaskFactory _taskFactory = new
            TaskFactory(CancellationToken.None,
                TaskCreationOptions.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);

        /// <summary>
        ///     Синхронный запуск
        /// </summary>
        /// <typeparam name="TResult">Результат выполнения задачи</typeparam>
        /// <param name="func">Исполняемый функтор</param>
        /// <returns></returns>
        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
            => _taskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

        /// <summary>
        ///     Синхронный запуск
        /// </summary>
        /// <param name="func">Исполняемый функтор</param>
        public static void RunSync(Func<Task> func)
            => _taskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
    }
}