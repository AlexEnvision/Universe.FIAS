using System;
using Universe.Diagnostic.Logger;
using Universe.Fias.Core.Infrastructure;
using Universe.Fias.Normalizer.Algorithms.AddressSystem.Utils;
using Universe.Fias.Normalizer.ConsoleApp.Tests.Base;

namespace Universe.Fias.Normalizer.ConsoleApp.Tests
{
	internal class FiasUtilTest : BaseTest
	{
        public FiasUtilTest(IUniverseLogger log) : base(log)
        {
        }

		public void Run()
		{
            var container = UnityConfig.Container;
            var settings = new AppSettings();

			settings.ConnectionString =
                "data source=localhost;initial catalog=FIAS-DB;integrated security=True;MultipleActiveResultSets=True;App=UniverseFiasApp;Connection Timeout=3600";

			var resolver = new AppPrincipalResolver();
            var scope = new UniverseFiasScope(resolver, settings, container);

            var searcher = new FiasUtil(scope);

			var info = searcher.GetFiasInfo("ул. Студенецкая, д.16а, корп.1");
			Log.Info($"Улица: {info.Street}, Дом: {info.House}");
		}
    }
}
