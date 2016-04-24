using System;
using System.Collections.Generic;
using System.Globalization;

namespace Ilaro.Admin.Core
{
    public class ValueConverter
    {
        readonly Func<string, Type, object> _defaultConverter =
            (value, type) => System.Convert.ChangeType(value, type, CultureInfo.CurrentCulture);

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

        //public object Convert(PropertyTypeInfo typeInfo, object value)
        //{
        //    var converter = GetConverter<typeInfo.Type>();
        //    //return (TOutput)converter(value, typeof(TOutput));


        //    if (value == null)
        //        return null;

        //    if (typeInfo.IsEnum)
        //        return GetConverter(typeInfo.EnumType Convert.ChangeType(Raw, typeInfo.EnumType, CultureInfo.CurrentCulture);
        //    if (typeInfo.IsNullable)
        //        return Convert.ChangeType(Raw, typeInfo.UnderlyingType, CultureInfo.CurrentCulture);
        //    if (typeInfo.IsFile)
        //        return Raw;

        //    return Convert.ChangeType(Raw, Property.TypeInfo.Type, CultureInfo.CurrentCulture);


        //    if (Raw == null)
        //        return null;

        //    if (Property.TypeInfo.IsEnum)
        //    {
        //        var enumValue =
        //            (Enum)Enum.Parse(Property.TypeInfo.EnumType, AsString);
        //        if (enumValue == null)
        //            return AsString;

        //        return enumValue;
        //    }
        //    if (Property.TypeInfo.IsNullable)
        //        return Convert.ChangeType(Raw, Property.TypeInfo.UnderlyingType);
        //    if (Property.TypeInfo.IsFile)
        //        return null;

        //    return Convert.ChangeType(Raw, Property.TypeInfo.Type);
        //}

        private Func<string, Type, object> GetConverter<TOutput>()
        {
            return GetConverter(typeof(TOutput));
        }

        private Func<string, Type, object> GetConverter(Type type)
        {
            Func<string, Type, object> converter = null;
            if (_convertersRegistry.TryGetValue(type, out converter) == false)
            {
                converter = _defaultConverter;
            }
            return converter;
        }
    }
}