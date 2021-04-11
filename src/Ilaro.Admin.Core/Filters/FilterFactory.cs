using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ilaro.Admin.Core.Extensions;
using Ilaro.Admin.Core.DataAccess;

namespace Ilaro.Admin.Core.Filters
{
    public class FilterFactory : IFilterFactory
    {
        private readonly IUser _user;
        private readonly IKnowTheTime _clock;

        public FilterFactory(IUser user, IKnowTheTime clock)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (clock == null)
                throw new ArgumentNullException(nameof(clock));

            _user = user;
            _clock = clock;
        }

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
                return _clock;

            var value = propertyValue.Raw;
            if (value == null)
                value = GetDefaultValue(propertyValue.Property.DefaultFilter);

            if (type == typeof(string))
                return value.ToStringSafe();

            return value;
        }

        private object GetDefaultValue(object value)
        {
            if (value is ValueBehavior)
            {
                switch ((ValueBehavior)value)
                {
                    case ValueBehavior.CurrentUserId:
                        return _user.Id();
                    case ValueBehavior.CurrentUserName:
                        return _user.UserName();
                    case ValueBehavior.Now:
                        return _clock.Now;
                    case ValueBehavior.UtcNow:
                        return _clock.UtcNow;
                }
            }

            return value;
        }
    }
}