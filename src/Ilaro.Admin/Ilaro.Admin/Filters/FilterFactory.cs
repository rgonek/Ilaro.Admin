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
        public IEnumerable<BaseFilter> BuildFilters(Entity entity)
        {
            var filters = GetAllFilters();

            foreach (var property in entity.Properties)
            {
                var filterType = GetMatchingFilter(property, filters);
                if (filterType == null)
                    continue;

                yield return CreateInstance(filterType, property);
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

            return null;
        }

        private IList<Type> GetAllFilters()
        {
            var baseFilterType = typeof(BaseFilter);
            return typeof(Admin).Assembly.GetTypes()
                .Where(x => baseFilterType.IsAssignableFrom(x) && x.BaseType != null && x.BaseType.IsGenericType)
                .ToList();
        }

        private BaseFilter CreateInstance(Type filterType, Property property)
        {
            var constructor = filterType.GetConstructors().Single();

            var args = GetArgs(constructor, property);

            return (BaseFilter)constructor.Invoke(args.ToArray());
        }

        private IEnumerable<object> GetArgs(ConstructorInfo constructor, Property property)
        {
            foreach (var parameter in constructor.GetParameters())
            {
                yield return GetArg(parameter.ParameterType, property);
            }
        }

        private object GetArg(Type type, Property property)
        {
            if (type == typeof(Property))
                return property;
            if (type == typeof(Entity))
                return property.Entity;
            if (type == typeof(IKnowTheTime))
                return new SystemClock();
            if (type == typeof(string))
                return property.Value.Raw.ToStringSafe();

            return property.Value.Raw;
        }
    }
}