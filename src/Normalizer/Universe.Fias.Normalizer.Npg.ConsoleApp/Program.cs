using System;
using Universe.CQRS;
using Universe.Diagnostic.Logger;
using Universe.Fias.Normalizer.Npg.ConsoleApp.Tests;
using Universe.Helpers.Extensions;

namespace Universe.Fias.Normalizer.Npg.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var log = new EventLogger();

            log.LogInfo += e => {
                if (e.AllowReport)
                {
                    var currentDate = DateTime.Now;
                    var message = $"[{currentDate}] {e.Message}{Environment.NewLine}";
                    Console.WriteLine(message);
                }
            };

            log.LogError += e => {
                if (e.AllowReport)
                {
                    var currentDate = DateTime.Now;
                    var message = $"[{currentDate}] Во время выполнения операции произошла ошибка. Текст ошибки: {e.Message}.{Environment.NewLine} Полный текст: {e.Ex.GetExceptionInfoMessageExceptionTypeStackTrace()}{Environment.NewLine}";
                    Console.WriteLine(message);
                }
            };

            log.LogWarning += e => {
                if (e.AllowReport)
                {
                    var currentDate = DateTime.Now;
                    var message = $"[{currentDate}] {e.Message}{Environment.NewLine}";
                    Console.WriteLine(message);
                }
            };

            MapperConfiguration.Configure();

            new NormalizerTest(log).Run();

            Console.WriteLine(@"Для продолжения нажмите любую клавишу...");
            Console.ReadLine();
        }
    }
}