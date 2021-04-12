using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Configuration.Configurators;
using Ilaro.Admin.Core.DataAccess;
using Ilaro.Admin.Core.File;
using Ilaro.Admin.Core.Filters;
using Ilaro.Admin.Core.Validation;
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
    public static class ServiceRegistrar
    {
        public static void AddIlaroAdminClasses(IServiceCollection services, IEnumerable<Assembly> assembliesToScan)
        {
            assembliesToScan = assembliesToScan.Distinct().ToArray();

            ConnectImplementationsToTypesClosing(typeof(IEntityConfigurator), services, assembliesToScan, true);
        }

        /// <summary>
        /// Helper method use to differentiate behavior between request handlers and notification handlers.
        /// Request handlers should only be added once (so set addIfAlreadyExists to false)
        /// Notification handlers should all be added (set addIfAlreadyExists to true)
        /// </summary>
        /// <param name="openRequestInterface"></param>
        /// <param name="services"></param>
        /// <param name="assembliesToScan"></param>
        /// <param name="addIfAlreadyExists"></param>
        private static void ConnectImplementationsToTypesClosing(Type openRequestInterface,
            IServiceCollection services,
            IEnumerable<Assembly> assembliesToScan,
            bool addIfAlreadyExists)
        {
            var concretions = new List<Type>();
            var interfaces = new List<Type>();
            foreach (var type in assembliesToScan.SelectMany(a => a.DefinedTypes).Where(t => !t.IsOpenGeneric()))
            {
                var interfaceTypes = type.FindInterfacesThatClose(openRequestInterface).ToArray();
                if (!interfaceTypes.Any()) continue;

                if (type.IsConcrete())
                {
                    concretions.Add(type);
                }

                foreach (var interfaceType in interfaceTypes)
                {
                    interfaces.Fill(interfaceType);
                }
            }

            foreach (var @interface in interfaces)
            {
                var exactMatches = concretions.Where(x => x.CanBeCastTo(@interface)).ToList();
                if (addIfAlreadyExists)
                {
                    foreach (var type in exactMatches)
                    {
                        services.AddTransient(@interface, type);
                    }
                }
                else
                {
                    if (exactMatches.Count > 1)
                    {
                        exactMatches.RemoveAll(m => !IsMatchingWithInterface(m, @interface));
                    }

                    foreach (var type in exactMatches)
                    {
                        services.TryAddTransient(@interface, type);
                    }
                }

                if (!@interface.IsOpenGeneric())
                {
                    AddConcretionsThatCouldBeClosed(@interface, concretions, services);
                }
            }
        }

        private static bool IsMatchingWithInterface(Type handlerType, Type handlerInterface)
        {
            if (handlerType == null || handlerInterface == null)
            {
                return false;
            }

            if (handlerType.IsInterface)
            {
                if (handlerType.GenericTypeArguments.SequenceEqual(handlerInterface.GenericTypeArguments))
                {
                    return true;
                }
            }
            else
            {
                return IsMatchingWithInterface(handlerType.GetInterface(handlerInterface.Name), handlerInterface);
            }

            return false;
        }

        private static void AddConcretionsThatCouldBeClosed(Type @interface, List<Type> concretions, IServiceCollection services)
        {
            foreach (var type in concretions
                .Where(x => x.IsOpenGeneric() && x.CouldCloseTo(@interface)))
            {
                try
                {
                    services.TryAddTransient(@interface, type.MakeGenericType(@interface.GenericTypeArguments));
                }
                catch (Exception)
                {
                }
            }
        }

        private static bool CouldCloseTo(this Type openConcretion, Type closedInterface)
        {
            var openInterface = closedInterface.GetGenericTypeDefinition();
            var arguments = closedInterface.GenericTypeArguments;

            var concreteArguments = openConcretion.GenericTypeArguments;
            return arguments.Length == concreteArguments.Length && openConcretion.CanBeCastTo(openInterface);
        }

        private static bool CanBeCastTo(this Type pluggedType, Type pluginType)
        {
            if (pluggedType == null) return false;

            if (pluggedType == pluginType) return true;

            return pluginType.GetTypeInfo().IsAssignableFrom(pluggedType.GetTypeInfo());
        }

        public static bool IsOpenGeneric(this Type type)
        {
            return type.GetTypeInfo().IsGenericTypeDefinition || type.GetTypeInfo().ContainsGenericParameters;
        }

        public static IEnumerable<Type> FindInterfacesThatClose(this Type pluggedType, Type templateType)
        {
            return FindInterfacesThatClosesCore(pluggedType, templateType).Distinct();
        }

        private static IEnumerable<Type> FindInterfacesThatClosesCore(Type pluggedType, Type templateType)
        {
            if (pluggedType == null) yield break;

            if (!pluggedType.IsConcrete()) yield break;

            if (templateType.GetTypeInfo().IsInterface)
            {
                var interfaces = pluggedType.GetInterfaces().ToList();
                var interfaces2 = interfaces.Where(type => type == templateType).ToList();
                foreach (
                    var interfaceType in
                    pluggedType.GetInterfaces()
                        .Where(type => type == templateType))
                {
                    yield return interfaceType;
                }
            }
            else if (pluggedType.GetTypeInfo().BaseType.GetTypeInfo().IsGenericType &&
                     (pluggedType.GetTypeInfo().BaseType.GetGenericTypeDefinition() == templateType))
            {
                yield return pluggedType.GetTypeInfo().BaseType;
            }

            if (pluggedType.GetTypeInfo().BaseType == typeof(object)) yield break;

            foreach (var interfaceType in FindInterfacesThatClosesCore(pluggedType.GetTypeInfo().BaseType, templateType))
            {
                yield return interfaceType;
            }
        }

        private static bool IsConcrete(this Type type)
        {
            return !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface;
        }

        private static void Fill<T>(this IList<T> list, T value)
        {
            if (list.Contains(value)) return;
            list.Add(value);
        }

        public static void AddRequiredServices(IServiceCollection services, IlaroAdminServiceConfiguration serviceConfiguration)
        {
            services.TryAdd<INotificator, Notificator>(serviceConfiguration.Lifetime);
            services.TryAdd<IKnowTheTime, SystemClock>(serviceConfiguration.Lifetime);
            services.TryAdd<IEntityService, EntityService>(serviceConfiguration.Lifetime);
            services.TryAdd<IValidatingEntities, EntityValidator>(serviceConfiguration.Lifetime);
            services.TryAdd<IValidatingFiles, FileValidator>(serviceConfiguration.Lifetime);
            services.TryAdd<IAppConfiguration, AppConfiguration>(serviceConfiguration.Lifetime);
            services.TryAdd<IRecordFetcher, RecordFetcher>(serviceConfiguration.Lifetime);
            services.TryAdd<IRecordHierarchyFetcher, RecordHierarchyFetcher>(serviceConfiguration.Lifetime);
            services.TryAdd<ICommandExecutor, CommandExecutor>(serviceConfiguration.Lifetime);
            services.TryAdd<IRecordCreator, RecordCreator>(serviceConfiguration.Lifetime);
            services.TryAdd<IRecordUpdater, RecordUpdater>(serviceConfiguration.Lifetime);
            services.TryAdd<IRecordDeleter, RecordDeleter>(serviceConfiguration.Lifetime);
            services.TryAdd<IRecordComparer, RecordComparer>(serviceConfiguration.Lifetime);
            services.TryAdd<IChangeDescriber, ChangeDescriber>(serviceConfiguration.Lifetime);
            services.TryAdd<IFileNameCreator, FileNameCreator>(serviceConfiguration.Lifetime);
            services.TryAdd<IFileDeleter, FileDeleter>(serviceConfiguration.Lifetime);
            services.TryAdd<IHandlingFiles, FileHandler>(serviceConfiguration.Lifetime);
            services.TryAdd<IResizingImages, ImageResizer>(serviceConfiguration.Lifetime);
            services.TryAdd<IHandlingFiles, FileHandler>(serviceConfiguration.Lifetime);
            services.TryAdd<ISavingFiles, FileSaver>(serviceConfiguration.Lifetime);
            services.TryAdd<IFilterFactory, FilterFactory>(serviceConfiguration.Lifetime);
            services.TryAdd<IRecordService, RecordService>(serviceConfiguration.Lifetime);
            services.TryAdd<IIlaroAdmin, IlaroAdmin>(serviceConfiguration.Lifetime);
            services.TryAdd<IUser, StubUser>(serviceConfiguration.Lifetime);
        }

        private static void TryAdd<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime)
            where TImplementation : class
            => services.TryAdd(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
    }
}
