using Ilaro.Admin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Filters
{
    public class FilterFactory : IFilterFactory
    {
        public IEnumerable<BaseFilter> BuildFilters(EntityRecord entityRecord)
        {
            var filters = GetAllFilters();

            foreach (var propertyValue in entityRecord.Values)
            {
                var filterType = GetMatchingFilter(propertyValue.Property, filters);
                if (filterType == null)
                    continue;

                yield return CreateInstance(filterType, propertyValue);
            }
        }

        private Type GetMatchingFilter(Property property, IList<Type> filters)
        {
            var filterType = filters.FirstOrDefault(x => x.BaseType.GetGenericArguments()[0] == property.TypeInfo.NotNullableType);
            if (filterType != null)
                return filterType;

            filterType = filters.FirstOrDefault(x => x.BaseType.GetGenericArguments()[0].IsAssignableFrom(property.TypeInfo.NotNullableType));
            if (filterType != null)
                return filterType;

            var groupers = filters.Where(x => typeof(ITypeGrouper).IsAssignableFrom(x.BaseType.GetGenericArguments()[0])).ToList();
            if (groupers.IsNullOrEmpty() == false)
                return groupers.FirstOrDefault(x => CreateTypeGrouperInstance(x.BaseType.GetGenericArguments()[0]).Match(property.TypeInfo.NotNullableType));

            return null;
        }

        private IList<Type> GetAllFilters()
        {
            var baseFilterType = typeof(BaseFilter);
            return typeof(Admin).Assembly.GetTypes()
                .Where(x =>
                    baseFilterType.IsAssignableFrom(x) &&
                    x.BaseType != null &&
                    x.BaseType.IsGenericType)
                .ToList();
        }

        private ITypeGrouper CreateTypeGrouperInstance(Type type)
        {
            return (ITypeGrouper)Activator.CreateInstance(type, null);
        }

        private BaseFilter CreateInstance(
            Type filterType,
            PropertyValue propertyValue)
        {
            var constructor = filterType.GetConstructors().Single();

            var args = GetArgs(constructor, propertyValue);

            return (BaseFilter)constructor.Invoke(args.ToArray());
        }

        private IEnumerable<object> GetArgs(
            ConstructorInfo constructor,
            PropertyValue propertyValue)
        {
            foreach (var parameter in constructor.GetParameters())
            {
                yield return GetArg(parameter.ParameterType, propertyValue);
            }
        }

        private object GetArg(
            Type type,
            PropertyValue propertyValue)
        {
            if (type == typeof(Property))
                return propertyValue.Property;
            if (type == typeof(Entity))
                return propertyValue.Property.Entity;
            if (type == typeof(IKnowTheTime))
                return new SystemClock();
            if (type == typeof(string))
                return propertyValue.Raw.ToStringSafe();

            return propertyValue.Raw;
        }
    }
}