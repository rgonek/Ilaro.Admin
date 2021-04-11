using Ilaro.Admin.Common;
using Ilaro.Admin.Common.Extensions;
using System.Collections.Generic;

namespace Ilaro.Admin.DataAccess
{
    public class PropertyValue
    {
        //private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(PropertyValue));
        private readonly ValueConverter _converter = new ValueConverter();

        public PropertyMetadata Metadata { get; }

        /// <summary>
        /// If property is a entity key.
        /// </summary>
        public bool IsKey { get; internal set; }

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
                    //if (Metadata.TypeInfo.IsFile && AsObject is HttpPostedFileWrapper)
                    //{
                    //    _asString = Raw.ToStringSafe();
                    //}
                    //else
                    {
                        var format = "";// Metadata.GetFormat();
                        _asString = format.HasValue()
                            ? string.Format("{0:" + format + "}", AsObject)
                            : AsObject.ToStringSafe();
                    }
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
                    _asObject = _converter.Convert(Metadata.TypeInfo, Raw);
                return _asObject;
            }
        }

        /// <summary>
        /// Possible values for foreign entity
        /// </summary>
        public IDictionary<string, string> PossibleValues { get; set; } = new Dictionary<string, string>();

        public string SqlParameterName { get; set; }

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