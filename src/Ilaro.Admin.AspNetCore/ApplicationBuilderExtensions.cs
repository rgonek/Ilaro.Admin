using Dawn;
using Ilaro.Admin.AspNetCore;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Configuration.Configurators;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension methods used to configure IlaroAdmin.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseIlaroAdmin(this IApplicationBuilder builder)
            => builder.UseIlaroAdmin(new IlaroAdminOptionsBuilder());

        public static IApplicationBuilder UseIlaroAdmin(
            this IApplicationBuilder builder,
            Action<IlaroAdminOptionsBuilder> configure)
        {
            var options = new IlaroAdminOptionsBuilder();
            configure(options);

            return builder.UseIlaroAdmin(options);
        }

        public static IApplicationBuilder UseIlaroAdmin(this IApplicationBuilder app, IlaroAdminOptionsBuilder optionsBuilder)
        {
            Guard.Argument(app, nameof(app)).NotNull();
            Guard.Argument(optionsBuilder, nameof(optionsBuilder)).NotNull();

            var options = app.ApplicationServices.GetService<IIlaroAdminOptions>();
            options.ConnectionString = optionsBuilder.ConnectionString;
            options.QueryFactoryFactory = optionsBuilder.QueryFactoryFactory;

            var entities = app.ApplicationServices.GetService<IEntityCollection>();
            var configurators = app.ApplicationServices.GetServices<IEntityConfigurator>();
            Configure(configurators, entities);

            return app;
        }

        private static void Configure(IEnumerable<IEntityConfigurator> configurators, IEntityCollection entities)
        {
            ConfigureEntities(configurators, entities);
            ConfigureProperties(configurators, entities);
        }

        private static void ConfigureEntities(IEnumerable<IEntityConfigurator> configurators, IEntityCollection entities)
        {
            foreach (var configurator in configurators)
            {
                var entity = new Entity(configurator.CustomizersHolder.Type);
                entities.Add(entity);
                ((ConfiguratorsHolder)configurator.CustomizersHolder).CustomizeEntity(entity);
            }
        }

        private static void ConfigureProperties(IEnumerable<IEntityConfigurator> configurators, IEntityCollection entities)
        {
            foreach (var configurator in configurators)
            {
                var entity = entities[configurator.CustomizersHolder.Type];
                ((ConfiguratorsHolder)configurator.CustomizersHolder).CustomizeProperties(entity, entities);
            }
        }
    }
}
