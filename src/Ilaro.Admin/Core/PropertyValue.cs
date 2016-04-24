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
        private readonly ValueConverter _converter = new ValueConverter();

        public DataBehavior DataBehavior { get; set; }
        public object Raw { get; set; }
        public object Additional { get; set; }
        public List<object> Values { get; set; } = new List<object>();
        private bool? _asBool;
        public bool? AsBool
        {
            get
            {
                if (_asBool == null)
                {
                    if (AsObject == null)
                        _asBool = null;
                    else if (AsObject is bool || AsObject is bool?)
                        _asBool = (bool?)AsObject;
                    else
                        _asBool = bool.Parse(AsObject.ToString());
                }
                return _asBool;
            }
        }
        private string _asString;
        public string AsString
        {
            get
            {
                if (_asString == null)
                {
                    var format = Property.GetFormat();
                    if (format.HasValue())
                        _asString = string.Format("{0:" + format + "}", AsObject);
                    else
                        _asString = AsObject.ToStringSafe();
                }
                return _asString;
            }
        }
        private object _asObject;
        public object AsObject
        {
            get
            {
                if (_asObject == null)
                    _asObject = _converter.Convert(Property.TypeInfo, Raw);
                return _asObject;
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