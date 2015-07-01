using System;
using System.Collections.Generic;
using System.Globalization;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Core
{
    public class PropertyValue
    {
        public object Raw { get; internal set; }
        public IList<object> Values { get; internal set; }
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

                if (_typeInfo.DataType == DataType.Numeric &&
                    _typeInfo.IsFloatingPoint)
                {
                    try
                    {
                        return Convert
                            .ToDecimal(Raw)
                            .ToString(CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        // ignored
                    }
                }

                return Raw.ToStringSafe();
            }
        }
        public object AsObject
        {
            get
            {
                if (_typeInfo.DataType == DataType.Enum)
                {
                    return Convert.ChangeType(Raw, _typeInfo.EnumType);
                }

                return Convert.ChangeType(Raw, _typeInfo.Type);
            }
        }

        /// <summary>
        /// Possible values for foreign entity
        /// </summary>
        public IDictionary<string, string> PossibleValues { get; internal set; }

        private readonly PropertyTypeInfo _typeInfo;

        public PropertyValue(PropertyTypeInfo typeInfo)
        {
            _typeInfo = typeInfo;
        }

        public object ToObject(string value)
        {
            Raw = value;
            return AsObject;
        }
    }
}