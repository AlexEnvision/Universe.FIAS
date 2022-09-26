using Universe.CQRS.Infrastructure;
using Universe.CQRS.Security.Principal;

namespace Universe.Fias.Core.Infrastructure
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class AppPrincipalResolver : IWebAppPrincipalResolver
    {
        public IWebAppPrincipal GetCurrentPrincipal()
        {
            return null;
        }
    }
}