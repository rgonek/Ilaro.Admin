using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc4;
using Ilaro.Admin.Services.Interfaces;
using Ilaro.Admin.Services;
using Ilaro.Admin.Commons.Notificator;
using Ilaro.Admin.Commons;

namespace Ilaro.Admin
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

		public static void RegisterTypes(IUnityContainer container)
		{
			container.RegisterType(typeof(Notificator), new UnityPerUserCacheLifetimeManager("Notificator"), new InjectionConstructor());

			container.RegisterType<IEntityService, EntityService>();
		}
	}
}