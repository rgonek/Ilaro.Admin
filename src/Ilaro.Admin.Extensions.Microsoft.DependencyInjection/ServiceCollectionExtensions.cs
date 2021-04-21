using Dawn;
using Ilaro.Admin.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ilaro.Admin.Extensions.Microsoft.DependencyInjection
{
    /// <summary>
    /// Extensions to scan for MediatR handlers and registers them.
    /// - Scans for any handler interface implementations and registers them as <see cref="ServiceLifetime.Transient"/>
    /// - Scans for any <see cref="IRequestPreProcessor{TRequest}"/> and <see cref="IRequestPostProcessor{TRequest,TResponse}"/> implementations and registers them as transient instances
    /// Registers <see cref="ServiceFactory"/> and <see cref="IMediator"/> as transient instances
    /// After calling AddMediatR you can use the container to resolve an <see cref="IMediator"/> instance.
    /// This does not scan for any <see cref="IPipelineBehavior{TRequest,TResponse}"/> instances including <see cref="RequestPreProcessorBehavior{TRequest,TResponse}"/> and <see cref="RequestPreProcessorBehavior{TRequest,TResponse}"/>.
    /// To register behaviors, use the <see cref="ServiceCollectionServiceExtensions.AddTransient(IServiceCollection,Type,Type)"/> with the open generic or closed generic types.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        private const string DefaultRoutePrefix = "/ilaroadmin";

        /// <summary>
        /// Registers configurations from the specified assemblies
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="routePrefix">Route prefix for administration site</param>
        /// <param name="assemblies">Assemblies to scan</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddIlaroAdmin(this IServiceCollection services, string routePrefix, IEnumerable<Assembly> assemblies)
        {
            Guard.Argument(assemblies, nameof(assemblies)).NotEmpty(x => "No assemblies found to scan. Supply at least one assembly to scan for handlers.");

            routePrefix ??= DefaultRoutePrefix;

            ServiceRegistrar.AddRequiredServices(services, routePrefix);
            ServiceRegistrar.AddIlaroAdminClasses(services, assemblies);

            services.Configure<RazorPagesOptions>(options =>
            {
                options.Conventions
                    .AddAreaPageRoute("IlaroAdmin", "/Index", routePrefix)
                    .AddAreaPageRoute("IlaroAdmin", "/Changes", routePrefix + "/{entity:alpha}/changes")
                    .AddAreaPageRoute("IlaroAdmin", "/Create", routePrefix + "/{entity:alpha}/new")
                    .AddAreaPageRoute("IlaroAdmin", "/Delete", routePrefix + "/{entity:alpha}/{id}/delete")
                    .AddAreaPageRoute("IlaroAdmin", "/Edit", routePrefix + "/{entity:alpha}/{id}/edit")
                    .AddAreaPageRoute("IlaroAdmin", "/List", routePrefix + "/{entity:alpha}");
            });

            services.Configure<MvcOptions>(options =>
            {
                options.ModelBinderProviders.Insert(0, new EntityModelBinderProvider());
                options.ModelBinderProviders.Insert(1, new IdValueModelBinderProvider());
            });

            services.ConfigureOptions<UiConfigureOptions>();

            return services;
        }

        /// <summary>
        /// Registers configurations from the specified assemblies
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="routePrefix">Route prefix for administration site</param>
        /// <param name="assemblies">Assemblies to scan</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddIlaroAdmin(this IServiceCollection services, string routePrefix, params Assembly[] assemblies)
            => services.AddIlaroAdmin(routePrefix, assemblies.AsEnumerable());

        /// <summary>
        /// Registers configurations from the specified assemblies
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="assemblies">Assemblies to scan</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddIlaroAdmin(this IServiceCollection services, params Assembly[] assemblies)
            => services.AddIlaroAdmin(DefaultRoutePrefix, assemblies.AsEnumerable());

        /// <summary>
        /// Registers configurations from the assemblies that contain the specified types
        /// </summary>
        /// <param name="services"></param>
        /// <param name="routePrefix">Route prefix for administration site</param>
        /// <param name="handlerAssemblyMarkerTypes"></param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddIlaroAdmin(this IServiceCollection services, string routePrefix, params Type[] handlerAssemblyMarkerTypes)
            => services.AddIlaroAdmin(routePrefix, handlerAssemblyMarkerTypes.AsEnumerable());

        /// <summary>
        /// Registers configurations from the assemblies that contain the specified types
        /// </summary>
        /// <param name="services"></param>
        /// <param name="handlerAssemblyMarkerTypes"></param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddIlaroAdmin(this IServiceCollection services, params Type[] handlerAssemblyMarkerTypes)
            => services.AddIlaroAdmin(DefaultRoutePrefix, handlerAssemblyMarkerTypes.AsEnumerable());

        /// <summary>
        /// Registers configurations from the assemblies that contain the specified types
        /// </summary>
        /// <param name="services"></param>
        /// <param name="routePrefix">Route prefix for administration site</param>
        /// <param name="handlerAssemblyMarkerTypes"></param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddIlaroAdmin(this IServiceCollection services, string routePrefix, IEnumerable<Type> handlerAssemblyMarkerTypes)
            => services.AddIlaroAdmin(routePrefix, handlerAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));

        /// <summary>
        /// Registers configurations from the assemblies that contain the specified types
        /// </summary>
        /// <param name="services"></param>
        /// <param name="handlerAssemblyMarkerTypes"></param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddIlaroAdmin(this IServiceCollection services, IEnumerable<Type> handlerAssemblyMarkerTypes)
            => services.AddIlaroAdmin(DefaultRoutePrefix, handlerAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));
    }
}
