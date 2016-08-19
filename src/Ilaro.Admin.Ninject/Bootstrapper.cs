using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Core.File;
using Ilaro.Admin.Core.Validation;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Mvc;
using System;
using System.Web;
using System.Web.Mvc;
using Ilaro.Admin.Core.Filters;
using NinjectBootstrapper = Ninject.Web.Common.Bootstrapper;

namespace Ilaro.Admin.Ninject
{
    public static class Bootstrapper
    {
        private static readonly NinjectBootstrapper bootstrapper = new NinjectBootstrapper();

        public static void Initialise()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        public static void ShutDown()
        {
            bootstrapper.ShutDown();
        }

        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new NinjectBootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterTypes(kernel);

                DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));

                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        private static void RegisterTypes(IKernel kernel)
        {
            kernel.Bind<Notificator>().ToSelf().InPerUserCacheScope();

            kernel.Bind<IKnowTheTime>().To<SystemClock>();
            kernel.Bind<IEntityService>().To<EntityService>();
            kernel.Bind<IValidatingEntities>().To<EntityValidator>();
            kernel.Bind<IValidatingFiles>().To<FileValidator>();
            kernel.Bind<IConfigurationProvider>().To<ConfigurationProvider>();
            kernel.Bind<IConfiguration>().To<Core.Configuration>();
            kernel.Bind<IFetchingRecords>().To<RecordsSource>();
            kernel.Bind<IFetchingRecordsHierarchy>().To<RecordsHierarchySource>();
            kernel.Bind<IExecutingDbCommand>().To<DbCommandExecutor>();
            kernel.Bind<ICreatingRecords>().To<RecordsCreator>();
            kernel.Bind<IUpdatingRecords>().To<RecordsUpdater>();
            kernel.Bind<IDeletingRecords>().To<RecordsDeleter>();
            kernel.Bind<IComparingRecords>().To<RecordsComparer>();
            kernel.Bind<IDescribingChanges>().To<ChangesDescriber>();
            kernel.Bind<IProvidingUser>().To<HttpContextUserProvider>();
            kernel.Bind<ICreatingNameFiles>().To<FileNameCreator>();
            kernel.Bind<IDeletingFiles>().To<FileDeleter>();
            kernel.Bind<IHandlingFiles>().To<FileHandler>();
            kernel.Bind<IResizingImages>().To<ImageResizer>();
            kernel.Bind<ISavingFiles>().To<FileSaver>();
            kernel.Bind<IFilterFactory>().To<FilterFactory>();
            kernel.Bind<IRecordsService>().To<RecordsService>();

            kernel.Bind<IIlaroAdmin>().To<IlaroAdmin>().InSingletonScope();
        }
    }
}
