using System;
using System.Web.Mvc;
using System.Globalization;
using System.Reflection;
using Ilaro.Admin.Registration;
using Ilaro.Admin.Configuration.Customizers;

namespace Ilaro.Admin
{
    public static class Admin
    {
        public static IIlaroAdmin Current
        {
            get
            {
                return DependencyResolver.Current.GetService<IIlaroAdmin>();
            }
        }

        public static EntityCustomizer<TEntity> RegisterEntity<TEntity>() where TEntity : class
        {
            return Current.RegisterEntity<TEntity>();
        }

        public static void RegisterEntity(Type entityType)
        {
            Current.RegisterEntity(entityType);
        }

        public static EntityCustomizer<TEntity> RegisterEntityWithAttributes<TEntity>() where TEntity : class
        {
            return Current.RegisterEntityWithAttributes<TEntity>();
        }

        public static void RegisterEntityWithAttributes(Type entityType)
        {
            Current.RegisterEntityWithAttributes(entityType);
        }

        public static IIlaroAdmin Initialise(
            string connectionStringName = "",
            string routesPrefix = "IlaroAdmin",
            IAuthorizationFilter authorize = null,
            CultureInfo culture = null)
        {
            var admin = Current;
            admin.Initialise(connectionStringName, routesPrefix, authorize);

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

        public static IRegisterCustomizers AssemblyCustomizers(params Assembly[] assemblies)
        {
            return ScanningRegistrationExtensions.RegisterAssemblyCustomizators(assemblies);
        }

        public static IRegisterCustomizers Customizers(params Type[] types)
        {
            return ScanningRegistrationExtensions.RegisterCustomizators(types);
        }

        internal static void AddCustomizer(ICustomizersHolder customizersHolder)
        {
            ((IlaroAdmin)Current).CustomizerHolders[customizersHolder.Type] = customizersHolder;
        }
    }
}