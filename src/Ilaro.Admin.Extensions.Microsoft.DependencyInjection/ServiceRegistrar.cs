using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Configuration.Configurators;
using Ilaro.Admin.Core.DataAccess;
using Ilaro.Admin.Core.File;
using Ilaro.Admin.Core.Filters;
using Ilaro.Admin.Core.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ilaro.Admin.Extensions.Microsoft.DependencyInjection
{
    public static class ServiceRegistrar
    {
        public static void AddIlaroAdminClasses(IServiceCollection services, IEnumerable<Assembly> assembliesToScan)
        {
            services.Scan(scan =>
                scan.FromAssemblies(assembliesToScan.Distinct().ToArray())
                    .AddClasses(classes => classes.AssignableTo<IEntityConfigurator>())
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());
        }

        public static void AddRequiredServices(IServiceCollection services, string routePrefix)
        {
            services.TryAddScoped(appServices =>
            {
                var options = appServices.GetService<IIlaroAdminOptions>();
                return options.QueryFactoryFactory(options.ConnectionString);
            });
            services.TryAddSingleton<IIlaroAdminOptions>(new IlaroAdminOptions
            {
                RoutePrefix = routePrefix
            });
            services.TryAddScoped<INotificator, Notificator>();
            services.TryAddScoped<IKnowTheTime, SystemClock>();
            services.TryAddScoped<IEntityService, EntityService>();
            services.TryAddScoped<IValidatingEntities, EntityValidator>();
            services.TryAddScoped<IValidatingFiles, FileValidator>();
            services.TryAddScoped<IAppConfiguration, AppConfiguration>();
            services.TryAddScoped<IRecordFetcher, RecordFetcher>();
            services.TryAddScoped<IRecordHierarchyFetcher, RecordHierarchyFetcher>();
            services.TryAddScoped<ICommandExecutor, CommandExecutor>();
            services.TryAddScoped<IRecordCreator, RecordCreator>();
            services.TryAddScoped<IRecordUpdater, RecordUpdater>();
            services.TryAddScoped<IRecordDeleter, RecordDeleter>();
            services.TryAddScoped<IRecordComparer, RecordComparer>();
            services.TryAddScoped<IChangeDescriber, ChangeDescriber>();
            services.TryAddScoped<IFileNameCreator, FileNameCreator>();
            services.TryAddScoped<IFileDeleter, FileDeleter>();
            services.TryAddScoped<IHandlingFiles, FileHandler>();
            services.TryAddScoped<IResizingImages, ImageResizer>();
            services.TryAddScoped<IHandlingFiles, FileHandler>();
            services.TryAddScoped<ISavingFiles, FileSaver>();
            services.TryAddScoped<IFilterFactory, FilterFactory>();
            services.TryAddScoped<IRecordService, RecordService>();
            services.TryAddScoped<IIlaroAdmin, IlaroAdmin>();
            services.TryAddScoped<IUser, StubUser>();
            services.TryAddSingleton<IEntityCollection, EntityCollection>();
        }
    }
}
