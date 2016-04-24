using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Models
{
    public class CellValue
    {
        private readonly ValueConverter _converter = new ValueConverter();

        public object Raw { get; set; }
        public Property Property { get; set; }

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

        public CellValue()
        {
        }

        public CellValue(Property property)
        {
            Property = property;
        }

        public CellValue(PropertyValue propertyValue)
        {
            Property = propertyValue.Property;
            Raw = propertyValue.Raw;
        }
    }
}
