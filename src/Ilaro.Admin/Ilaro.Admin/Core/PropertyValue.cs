using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Ilaro.Admin.DataAnnotations;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Core
{
    public class PropertyValue
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(PropertyValue));

        public object Raw { get; set; }
        public object Additional { get; set; }
        public List<object> Values { get; set; }
        public bool? AsBool
        {
            get
            {
                if (Raw == null)
                {
                    return null;
                }

                if (Raw is bool || Raw is bool?)
                {
                    return (bool?)Raw;
                }
                else if (Raw is string)
                {
                    return bool.Parse(Raw.ToString());
                }

                return null;
            }
        }
        public string AsString
        {
            get
            {
                if (Raw == null)
                {
                    return String.Empty;
                }

                if (_typeInfo.IsNumber)
                {
                    try
                    {
                        return Convert
                            .ToDecimal(Raw)
                            .ToString(CultureInfo.InvariantCulture);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                    }
                }

                return Raw.ToStringSafe();
            }
        }
        public object AsObject
        {
            get
            {
                if (_typeInfo.IsEnum)
                    return Convert.ChangeType(Raw, _typeInfo.EnumType);
                if (_typeInfo.IsNullable)
                    return Convert.ChangeType(Raw, _typeInfo.UnderlyingType);
                if (_typeInfo.IsFile)
                    return null;

                return Convert.ChangeType(Raw, _typeInfo.Type);
            }
        }

        /// <summary>
        /// Possible values for foreign entity
        /// </summary>
        public IDictionary<string, string> PossibleValues { get; set; }

        public object DefaultValue { get; private set; }

        private readonly PropertyTypeInfo _typeInfo;

        public PropertyValue(object[] attributes, PropertyTypeInfo typeInfo)
        {
            _typeInfo = typeInfo;
            Values = new List<object>();

            var defaultValueAttribute = attributes
                .FirstOrDefault(x =>
                    x.GetType() == typeof(DefaultValueAttribute)) as DefaultValueAttribute;
            if (defaultValueAttribute != null)
                DefaultValue = defaultValueAttribute.Value;
        }

        public object ToObject(string value)
        {
            Raw = value;
            return AsObject;
        }

        public void Clear()
        {
            Raw = null;
            Values = new List<object>();
            PossibleValues = new Dictionary<string, string>();
        }
    }
}