using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Registration
{
    static class ScanningRegistrationExtensions
    {
        public static RegistrationBuilder
               RegisterAssemblyTypes(params Assembly[] assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            var rb = new RegistrationBuilder();

            rb.RegisterCallback(() => ScanAssemblies(assemblies, rb));

            return rb;
        }

        public static RegistrationBuilder
               RegisterTypes(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            var rb = new RegistrationBuilder();

            rb.RegisterCallback(() => ScanTypes(types, rb));

            return rb;
        }

        static void ScanAssemblies(IEnumerable<Assembly> assemblies, RegistrationBuilder rb)
        {
            ScanTypes(assemblies.SelectMany(a => a.GetLoadableTypes()), rb);
        }

        static void ScanTypes(IEnumerable<Type> types, RegistrationBuilder rb)
        {
            foreach (var t in types
                .Where(t => t.GetTypeInfo().IsClass &&
                !t.GetTypeInfo().IsAbstract &&
                !t.GetTypeInfo().IsGenericTypeDefinition &&
                !t.IsDelegate() &&
                rb.Filters.All(p => p(t))))
            {
                Admin.RegisterEntity(t);
            }
        }
    }
}