using System.Web.Mvc;
using Ilaro.Admin.Commons;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Services;
using Ilaro.Admin.Validation;
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
            container.RegisterType<IValidateEntity, EntityValidator>();
            container.RegisterType<IConfigurationProvider, ConfigurationProvider>();
            container.RegisterType<IConfiguration, Configuration>();
            container.RegisterType<IFetchingRecords, RecordsSource>();
            container.RegisterType<IFetchingRecordsHierarchy, RecordsHierarchySource>();
            container.RegisterType<IExecutingDbCommand, DbCommandExecutor>();
            container.RegisterType<ICreatingRecords, RecordsCreator>();
            container.RegisterType<IUpdatingRecords, RecordsUpdater>();
            container.RegisterType<IDeletingRecords, RecordsDeleter>();
            container.RegisterType<IProvidingUser, HttpContextUserProvider>();
        }
    }
}