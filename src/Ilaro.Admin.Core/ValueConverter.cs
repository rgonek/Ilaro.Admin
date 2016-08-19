using System;
using System.Collections.Generic;
using System.Globalization;

namespace Ilaro.Admin.Core
{
    public class ValueConverter
    {
        readonly Func<object, Type, object> _defaultConverter =
            (value, type) => System.Convert.ChangeType(value, type, CultureInfo.CurrentCulture);

        readonly Dictionary<Type, Func<object, Type, object>> _convertersRegistry =
            new Dictionary<Type, Func<object, Type, object>>
            {
                { typeof(TimeSpan), (value, type) => TimeSpan.Parse(value.ToString()) },
                { typeof(DateTime?), (value, type) => value == null ? (DateTime?)null : DateTime.Parse(value.ToString()) },
                { typeof(DateTime), (value, type) => DateTime.Parse(value.ToString()) },
                { typeof(int), (value, type) => int.Parse(value.ToString()) },
            };

        public TOutput GetAs<TOutput>(string value)
        {
            var converter = GetConverter<TOutput>();
            return (TOutput)converter(value, typeof(TOutput));
        }

        public object Convert(PropertyTypeInfo typeInfo, object value)
        {
            if (value == null)
                return null;

            if (typeInfo.IsFile || typeInfo.IsSystemType == false)
                return value;

            var type = typeInfo.GetPropertyType();
            if (typeInfo.IsEnum)
            {
                var enumValue = (Enum)Enum.Parse(type, value.ToString());
                if (enumValue == null)
                    return value.ToString();

                return enumValue;
            }
            var converter = GetConverter(type);

            return converter(value, type);
        }

        private Func<object, Type, object> GetConverter<TOutput>()
        {
            return GetConverter(typeof(TOutput));
        }

        private Func<object, Type, object> GetConverter(Type type)
        {
            Func<object, Type, object> converter = null;
            if (_convertersRegistry.TryGetValue(type, out converter) == false)
            {
                converter = _defaultConverter;
            }
            return converter;
        }
    }
}