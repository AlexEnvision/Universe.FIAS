using Universe.Diagnostic.Logger;

namespace Universe.Fias.Normalizer.Npg.ConsoleApp.Tests.Base
{
    internal abstract class BaseTest
    {
        protected readonly IUniverseLogger Log;

        public BaseTest(IUniverseLogger log)
        {
            Log = log;
        }
    }
}
