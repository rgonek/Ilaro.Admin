using System.Web.Mvc;
using Ilaro.Admin.Commons;
using Ilaro.Admin.Core;
using Ilaro.Admin.Services;
using Ilaro.Admin.Services.Interfaces;
using Microsoft.Practices.Unity;
using Unity.Mvc4;

namespace Ilaro.Admin.Unity
{
    public static class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            return container;
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            RegisterTypes(container);

            return container;
        }

        private static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType(
                typeof(Notificator),
                new PerUserCacheLifetimeManager(),
                new InjectionConstructor());
            container.RegisterType<IEntityService, EntityService>();
        }
    }
}