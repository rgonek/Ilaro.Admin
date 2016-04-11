using System;
using System.Collections.Generic;
using Ilaro.Admin.Extensions;
using System.Globalization;
using Ilaro.Admin.Core.Data;

namespace Ilaro.Admin.Core
{
    public class PropertyValue
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(PropertyValue));

        public DataBehavior DataBehavior { get; set; }
        public object Raw { get; set; }
        public object Additional { get; set; }
        public List<object> Values { get; set; } = new List<object>();
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

                if (Property.TypeInfo.IsNumber)
                {
                    try
                    {
                        return Convert
                            .ToDecimal(Raw)
                            .ToString(CultureInfo.CurrentCulture);
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
                if (Property.TypeInfo.IsEnum)
                    return Convert.ChangeType(Raw, Property.TypeInfo.EnumType, CultureInfo.CurrentCulture);
                if (Property.TypeInfo.IsNullable)
                    return Convert.ChangeType(Raw, Property.TypeInfo.UnderlyingType, CultureInfo.CurrentCulture);
                if (Property.TypeInfo.IsFile)
                    return null;
                if (Raw == null)
                    return null;

                return Convert.ChangeType(Raw, Property.TypeInfo.Type, CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Possible values for foreign entity
        /// </summary>
        public IDictionary<string, string> PossibleValues { get; set; } = new Dictionary<string, string>();

        public string SqlParameterName { get; set; }

        public Property Property { get; }

        public PropertyValue(Property property)
        {
            Property = property;
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