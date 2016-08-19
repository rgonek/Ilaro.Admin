using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Core.File;
using Ilaro.Admin.Core.Filters;
using Ilaro.Admin.Core.Validation;

namespace Ilaro.Admin.Autofac
{
    public static class Bootstrapper
    {
        public static IContainer Initialise()
        {
            var container = BuildContainer();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            return container;
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            RegisterTypes(builder);

            return builder.Build();
        }

        private static void RegisterTypes(ContainerBuilder builder)
        {
            builder.RegisterControllers(typeof(IlaroAdmin).Assembly);

            builder.RegisterType<Notificator>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<SystemClock>().As<IKnowTheTime>();
            builder.RegisterType<EntityService>().As<IEntityService>();
            builder.RegisterType<EntityValidator>().As<IValidatingEntities>();
            builder.RegisterType<FileValidator>().As<IValidatingFiles>();
            builder.RegisterType<ConfigurationProvider>().As<IConfigurationProvider>();
            builder.RegisterType<Core.Configuration>().As<IConfiguration>();
            builder.RegisterType<RecordsSource>().As<IFetchingRecords>();
            builder.RegisterType<RecordsHierarchySource>().As<IFetchingRecordsHierarchy>();
            builder.RegisterType<DbCommandExecutor>().As<IExecutingDbCommand>();
            builder.RegisterType<RecordsCreator>().As<ICreatingRecords>();
            builder.RegisterType<RecordsUpdater>().As<IUpdatingRecords>();
            builder.RegisterType<RecordsDeleter>().As<IDeletingRecords>();
            builder.RegisterType<RecordsComparer>().As<IComparingRecords>();
            builder.RegisterType<ChangesDescriber>().As<IDescribingChanges>();
            builder.RegisterType<HttpContextUserProvider>().As<IProvidingUser>();
            builder.RegisterType<FileNameCreator>().As<ICreatingNameFiles>();
            builder.RegisterType<FileDeleter>().As<IDeletingFiles>();
            builder.RegisterType<FileHandler>().As<IHandlingFiles>();
            builder.RegisterType<ImageResizer>().As<IResizingImages>();
            builder.RegisterType<FileSaver>().As<ISavingFiles>();
            builder.RegisterType<FilterFactory>().As<IFilterFactory>();
            builder.RegisterType<RecordsService>().As<IRecordsService>();

            builder.RegisterType<IlaroAdmin>().As<IIlaroAdmin>().SingleInstance();
        }
    }
}
