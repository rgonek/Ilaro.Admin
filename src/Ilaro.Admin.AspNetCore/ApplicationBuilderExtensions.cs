using Dawn;
using Ilaro.Admin.AspNetCore;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Configuration.Configurators;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using IlaroAdminOptions = Ilaro.Admin.AspNetCore.IlaroAdminOptions;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension methods used to add the middleware to the HTTP request pipeline.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        private static readonly string _defaultPath = "/ilaroadmin";

        public static IApplicationBuilder UseIlaroAdmin(this IApplicationBuilder builder)
            => builder.UseIlaroAdmin(null, new IlaroAdminOptions());

        public static IApplicationBuilder UseIlaroAdmin(
            this IApplicationBuilder builder,
            string path)
            => builder.UseIlaroAdmin(path, new IlaroAdminOptions());

        public static IApplicationBuilder UseIlaroAdmin(
            this IApplicationBuilder builder,
            string path,
            Action<IlaroAdminOptions> configure)
        {
            var options = new IlaroAdminOptions();
            configure(new IlaroAdminOptions());

            return builder.UseIlaroAdmin(path, options);
        }

        public static IApplicationBuilder UseIlaroAdmin(
            this IApplicationBuilder builder,
            Action<IlaroAdminOptions> configure)
        {
            var options = new IlaroAdminOptions();
            configure(options);

            return builder.UseIlaroAdmin(_defaultPath, options);
        }

        public static IApplicationBuilder UseIlaroAdmin(
            this IApplicationBuilder builder,
            IlaroAdminOptions options)
            => builder.UseIlaroAdmin(_defaultPath, options);

        public static IApplicationBuilder UseIlaroAdmin(this IApplicationBuilder app, string path, IlaroAdminOptions options)
        {
            Guard.Argument(app, nameof(app)).NotNull();
            Guard.Argument(options, nameof(options)).NotNull();

            var options2 = app.ApplicationServices.GetService<IIlaroAdminOptions>();
            var entities = app.ApplicationServices.GetService<IEntityCollection>();
            var configurators = app.ApplicationServices.GetServices<IEntityConfigurator>();
            Configure(configurators, entities);
            options2.ConnectionStringName = options.ConnectionStringName;

            return app.UseWhen(x => x.Request.Path.StartsWithSegments(path), b => b.UseMiddleware<IlaroAdminMiddleware>(options));
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
