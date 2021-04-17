using Dawn;
using Ilaro.Admin.Core;
using Ilaro.Admin.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ilaro.Admin.Extensions.Microsoft.DependencyInjection
{
    // Copied from MediatR
    // some works still need to be done
    // descriptions need to be changed
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
        /// <summary>
        /// Registers handlers and mediator types from the specified assemblies
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="assemblies">Assemblies to scan</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddIlaroAdmin(this IServiceCollection services, string routePrefix, params Assembly[] assemblies)
            => services.AddIlaroAdmin(routePrefix, assemblies, configuration: null);

        /// <summary>
        /// Registers handlers and mediator types from the specified assemblies
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="assemblies">Assemblies to scan</param>
        /// <param name="configuration">The action used to configure the options</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddIlaroAdmin(this IServiceCollection services, string routePrefix, Action<IlaroAdminServiceConfiguration> configuration, params Assembly[] assemblies)
            => services.AddIlaroAdmin(routePrefix, assemblies, configuration);

        /// <summary>
        /// Registers handlers and mediator types from the specified assemblies
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="assemblies">Assemblies to scan</param>
        /// <param name="configuration">The action used to configure the options</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddIlaroAdmin(this IServiceCollection services, string routePrefix, IEnumerable<Assembly> assemblies, Action<IlaroAdminServiceConfiguration> configuration)
        {
            Guard.Argument(assemblies, nameof(assemblies)).NotEmpty(x => "No assemblies found to scan. Supply at least one assembly to scan for handlers.");

            var serviceConfig = new IlaroAdminServiceConfiguration();
            configuration?.Invoke(serviceConfig);

            ServiceRegistrar.AddRequiredServices(services, serviceConfig);
            ServiceRegistrar.AddIlaroAdminClasses(services, assemblies);

            IIlaroAdminOptions appOptions = new IlaroAdminOptions
            {
                RoutePrefix = routePrefix
            };
            services.TryAddSingleton(appOptions);

            services.Configure<RazorPagesOptions>(options =>
            {
                options.Conventions
                    .AddAreaPageRoute("IlaroAdmin", "/Index", routePrefix)
                    .AddAreaPageRoute("IlaroAdmin", "/Changes", routePrefix + "/{entity:alpha}/changes")
                    .AddAreaPageRoute("IlaroAdmin", "/Create", routePrefix + "/{entity:alpha}/new")
                    .AddAreaPageRoute("IlaroAdmin", "/Delete", routePrefix + "/{entity:alpha}/{id}/delete")
                    .AddAreaPageRoute("IlaroAdmin", "/Details", routePrefix + "/{entity:alpha}/{id}")
                    .AddAreaPageRoute("IlaroAdmin", "/Edit", routePrefix + "/{entity:alpha}/{id}/edit")
                    .AddAreaPageRoute("IlaroAdmin", "/List", routePrefix + "/{entity:alpha}")
                    .AddAreaPageRoute("IlaroAdmin", "/GroupDetails", routePrefix + "/group/{entity:alpha}");
            });

            services.Configure<MvcOptions>(options =>
            {
                options.ModelBinderProviders.Insert(0, new EntityModelBinderProvider());
            });


            services.ConfigureOptions<UiConfigureOptions>();

            return services;
        }

        /// <summary>
        /// Registers handlers and mediator types from the assemblies that contain the specified types
        /// </summary>
        /// <param name="services"></param>
        /// <param name="handlerAssemblyMarkerTypes"></param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddIlaroAdmin(this IServiceCollection services, string routePrefix, params Type[] handlerAssemblyMarkerTypes)
            => services.AddIlaroAdmin(routePrefix, handlerAssemblyMarkerTypes, configuration: null);

        /// <summary>
        /// Registers handlers and mediator types from the assemblies that contain the specified types
        /// </summary>
        /// <param name="services"></param>
        /// <param name="handlerAssemblyMarkerTypes"></param>
        /// <param name="configuration">The action used to configure the options</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddIlaroAdmin(this IServiceCollection services, string routePrefix, Action<IlaroAdminServiceConfiguration> configuration, params Type[] handlerAssemblyMarkerTypes)
            => services.AddIlaroAdmin(routePrefix, handlerAssemblyMarkerTypes, configuration);

        /// <summary>
        /// Registers handlers and mediator types from the assemblies that contain the specified types
        /// </summary>
        /// <param name="services"></param>
        /// <param name="handlerAssemblyMarkerTypes"></param>
        /// <param name="configuration">The action used to configure the options</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddIlaroAdmin(this IServiceCollection services, string routePrefix, IEnumerable<Type> handlerAssemblyMarkerTypes, Action<IlaroAdminServiceConfiguration> configuration)
            => services.AddIlaroAdmin(routePrefix, handlerAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly), configuration);
    }
}
