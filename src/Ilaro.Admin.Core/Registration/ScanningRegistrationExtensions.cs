using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ilaro.Admin.Core.Extensions;
using Ilaro.Admin.Core.Configuration.Configurators;

namespace Ilaro.Admin.Core.Registration
{
    internal static class ScanningRegistrationExtensions
    {
        internal static IRegisterTypes RegisterAssemblyTypes(params Assembly[] assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            var rb = new RegistrationBuilder();
            rb.Where(type => !typeof(IEntityConfigurator).IsAssignableFrom(type));

            rb.RegisterCallback(customizerMutator => ScanAssemblies(assemblies, rb, customizerMutator));

            return rb;
        }

        internal static IRegisterTypes RegisterTypes(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            var rb = new RegistrationBuilder();
            rb.Where(type => !typeof(IEntityConfigurator).IsAssignableFrom(type));

            rb.RegisterCallback(customizerMutator => ScanTypes(types, rb, customizerMutator));

            return rb;
        }

        internal static IRegisterCustomizers RegisterAssemblyCustomizators(params Assembly[] assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            var rb = new RegistrationBuilder();
            rb.Where(type => typeof(IEntityConfigurator).IsAssignableFrom(type));

            rb.RegisterCallback(customizerMutator => ScanAssembliesCustomizators(assemblies, rb));

            return rb;
        }

        internal static IRegisterCustomizers RegisterCustomizators(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            var rb = new RegistrationBuilder();
            rb.Where(type => typeof(IEntityConfigurator).IsAssignableFrom(type));

            rb.RegisterCallback(customizerMutator => ScanTypesCustomizators(types, rb));

            return rb;
        }

        private static void ScanAssemblies(
            IEnumerable<Assembly> assemblies,
            RegistrationBuilder rb,
            Action<IConfiguratorsHolder> customizerMutator)
        {
            ScanTypes(assemblies.SelectMany(a => a.GetLoadableTypes()), rb, customizerMutator);
        }

        private static void ScanTypes(
            IEnumerable<Type> types,
            RegistrationBuilder rb,
            Action<IConfiguratorsHolder> customizerMutator)
        {
            foreach (var type in GetTypes(types, rb))
            {
                var customizerHolder = new ConfiguratorsHolder(type);
                if (customizerMutator != null)
                    customizerMutator(customizerHolder);

                Admin.AddConfigurator(customizerHolder);
            }
        }

        private static void ScanAssembliesCustomizators(
            IEnumerable<Assembly> assemblies,
            RegistrationBuilder rb)
        {
            ScanTypesCustomizators(assemblies.SelectMany(a => a.GetLoadableTypes()), rb);
        }

        private static void ScanTypesCustomizators(
            IEnumerable<Type> types,
            RegistrationBuilder rb)
        {
            foreach (var type in GetTypes(types, rb))
            {
                var customizer = GetCustomizerInstance(type);

                Admin.AddConfigurator(customizer.CustomizersHolder);
            }
        }

        private static IEntityConfigurator GetCustomizerInstance(Type type)
        {
            object customizerInstance;
            try
            {
                customizerInstance = Activator.CreateInstance(type);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Unable to instantiate configurator class (see InnerException): " + type, e);
            }

            var customizer = customizerInstance as IEntityConfigurator;
            if (customizer == null)
            {
                throw new ArgumentOutOfRangeException("type", "The configurator class must be an implementation of IEntityCustomizer.");
            }

            return customizer;
        }

        private static IEnumerable<Type> GetTypes(IEnumerable<Type> types, RegistrationBuilder rb)
        {
            return types
                .Where(t =>
                    t.TypeIsPublicClass() &&
                    !t.GetTypeInfo().IsGenericTypeDefinition &&
                    !t.IsDelegate() &&
                    t.GetConstructor(Type.EmptyTypes) != null &&
                    rb.Filters.All(p => p(t)));
        }
    }
}