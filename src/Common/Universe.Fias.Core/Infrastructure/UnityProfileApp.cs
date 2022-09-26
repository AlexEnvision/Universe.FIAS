using Unity;
using Unity.Lifetime;
using Universe.CQRS.Infrastructure;

namespace Universe.Fias.Core.Infrastructure
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class UnityProfileApp
    {
        private readonly IUnityContainer _container;

        public UnityProfileApp(IUnityContainer container)
        {
            _container = container;
        }

        public virtual void Apply()
        {
            // Infrastructure
            _container.RegisterType<IAppSettings, AppSettings>(new HierarchicalLifetimeManager());
            _container.RegisterType<IWebAppPrincipalResolver, AppPrincipalResolver>(new SingletonLifetimeManager());
        }
    }
}