using System.Web.Mvc;
using Ilaro.Admin.Commons;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Core.File;
using Ilaro.Admin.Filters;
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
            container.RegisterType<IValidatingEntities, EntityValidator>();
            container.RegisterType<IValidatingFiles, FileValidator>();
            container.RegisterType<IConfigurationProvider, ConfigurationProvider>();
            container.RegisterType<IConfiguration, Core.Configuration>();
            container.RegisterType<IFetchingRecords, RecordsSource>();
            container.RegisterType<IFetchingRecordsHierarchy, RecordsHierarchySource>();
            container.RegisterType<IExecutingDbCommand, DbCommandExecutor>();
            container.RegisterType<ICreatingRecords, RecordsCreator>();
            container.RegisterType<IUpdatingRecords, RecordsUpdater>();
            container.RegisterType<IDeletingRecords, RecordsDeleter>();
            container.RegisterType<IComparingRecords, RecordsComparer>();
            container.RegisterType<IDescribingChanges, ChangesDescriber>();
            container.RegisterType<IProvidingUser, HttpContextUserProvider>();
            container.RegisterType<ICreatingNameFiles, FileNameCreator>();
            container.RegisterType<IDeletingFiles, FileDeleter>();
            container.RegisterType<IHandlingFiles, FileHandler>();
            container.RegisterType<IResizingImages, ImageResizer>();
            container.RegisterType<ISavingFiles, FileSaver>();
            container.RegisterType<IFilterFactory, FilterFactory>();
            container.RegisterType<IRecordsService, RecordsService>();

            container.RegisterType<IIlaroAdmin, IlaroAdmin>(new ContainerControlledLifetimeManager());
        }
    }
}