using System;
using System.Collections.Generic;

namespace Ilaro.Admin.Core
{
    public class ValueConverter
    {
        readonly Func<string, Type, object> _defaultConverter =
            (value, type) => Convert.ChangeType(value, type);

        readonly Dictionary<Type, Func<string, Type, object>> _convertersRegistry =
            new Dictionary<Type, Func<string, Type, object>>
            {
                { typeof(TimeSpan), (value, type) => TimeSpan.Parse(value) },
                { typeof(DateTime?), (value, type) => String.IsNullOrWhiteSpace(value) ? (DateTime?)null : DateTime.Parse(value) },
                { typeof(DateTime), (value, type) => DateTime.Parse(value) },
                { typeof(int), (value, type) => int.Parse(value) },
            };

        public TOutput GetAs<TOutput>(string value)
        {
            var converter = GetConverter<TOutput>();
            return (TOutput)converter(value, typeof(TOutput));
        }

        private Func<string, Type, object> GetConverter<TOutput>()
        {
            Func<string, Type, object> converter = null;
            if (_convertersRegistry.TryGetValue(typeof(TOutput), out converter) == false)
            {
                converter = _defaultConverter;
            }
            return converter;
        }
    }
}