using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Configuration.Customizers;

namespace Ilaro.Admin.Registration
{
    static class ScanningRegistrationExtensions
    {
        public static RegistrationBuilder RegisterAssemblyTypes(params Assembly[] assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            var rb = new RegistrationBuilder();
            rb.Where(type => !typeof(IEntityCustomizer).IsAssignableFrom(type));

            rb.RegisterCallback(() => ScanAssemblies(assemblies, rb));

            return rb;
        }

        public static RegistrationBuilder RegisterTypes(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            var rb = new RegistrationBuilder();
            rb.Where(type => !typeof(IEntityCustomizer).IsAssignableFrom(type));

            rb.RegisterCallback(() => ScanTypes(types, rb));

            return rb;
        }

        public static RegistrationBuilder RegisterAssemblyCustomizators(params Assembly[] assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            var rb = new RegistrationBuilder();
            rb.Where(type => typeof(IEntityCustomizer).IsAssignableFrom(type));

            rb.RegisterCallback(() => ScanAssembliesCustomizators(assemblies, rb));

            return rb;
        }

        public static RegistrationBuilder RegisterCustomizators(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            var rb = new RegistrationBuilder();
            rb.Where(type => typeof(IEntityCustomizer).IsAssignableFrom(type));

            rb.RegisterCallback(() => ScanTypesCustomizators(types, rb));

            return rb;
        }

        private static void ScanAssemblies(IEnumerable<Assembly> assemblies, RegistrationBuilder rb)
        {
            ScanTypes(assemblies.SelectMany(a => a.GetLoadableTypes()), rb);
        }

        private static void ScanTypes(IEnumerable<Type> types, RegistrationBuilder rb)
        {
            foreach (var type in GetTypes(types, rb))
            {
                Admin.RegisterEntity(type);
            }
        }

        private static void ScanAssembliesCustomizators(IEnumerable<Assembly> assemblies, RegistrationBuilder rb)
        {
            ScanTypesCustomizators(assemblies.SelectMany(a => a.GetLoadableTypes()), rb);
        }

        private static void ScanTypesCustomizators(IEnumerable<Type> types, RegistrationBuilder rb)
        {
            foreach (var type in GetTypes(types, rb))
            {
                var customizer = GetCustomizerInstance(type);

                var entity = Admin.RegisterEntity(customizer.CustomizersHolder.Type);
                customizer.CustomizersHolder.CustomizeEntity(entity);
            }
        }

        private static IEntityCustomizer GetCustomizerInstance(Type type)
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

            var customizer = customizerInstance as IEntityCustomizer;
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