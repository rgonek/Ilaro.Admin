using System;
using System.Globalization;
using System.Reflection;
using Ilaro.Admin.Core.Registration;
using Ilaro.Admin.Core.Configuration.Configurators;
using Microsoft.AspNetCore.Authorization;

namespace Ilaro.Admin.Core
{
    public static class Admin
    {
        public static IIlaroAdmin Current
        {
            get
            {
                return null;
                //return DependencyResolver.Current.GetService<IIlaroAdmin>();
            }
        }

        public static EntityConfigurator<TEntity> RegisterEntity<TEntity>() where TEntity : class
        {
            return Current.RegisterEntity<TEntity>();
        }

        public static void RegisterEntity(Type entityType)
        {
            Current.RegisterEntity(entityType);
        }

        public static IIlaroAdmin Initialise(
            string connectionStringName = "",
            string routesPrefix = "IlaroAdmin",
            CultureInfo culture = null)
        {
            var admin = Current;
            admin.Initialise(connectionStringName, routesPrefix);

            return admin;
        }

        public static IRegisterTypes AssemblyEntities(params Assembly[] assemblies)
        {
            return ScanningRegistrationExtensions.RegisterAssemblyTypes(assemblies);
        }

        public static IRegisterTypes Entities(params Type[] types)
        {
            return ScanningRegistrationExtensions.RegisterTypes(types);
        }

        public static IRegisterCustomizers AssemblyConfigurators(params Assembly[] assemblies)
        {
            return ScanningRegistrationExtensions.RegisterAssemblyCustomizators(assemblies);
        }

        public static IRegisterCustomizers Configurators(params Type[] types)
        {
            return ScanningRegistrationExtensions.RegisterCustomizators(types);
        }

        internal static void AddConfigurator(IConfiguratorsHolder customizersHolder)
        {
            ((IlaroAdmin)Current).CustomizerHolders[customizersHolder.Type] = customizersHolder;
        }
    }
}