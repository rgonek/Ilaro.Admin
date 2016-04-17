using System;
using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Models
{
    public class CellValue
    {
        public object Raw { get; set; }

        public Property Property { get; set; }

        public string AsString
        {
            get { return Raw.ToStringSafe(Property); }
        }

        public bool? AsBool
        {
            get
            {
                if (AsString.IsNullOrEmpty())
                {
                    return null;
                }

                return bool.Parse(AsString);
            }
        }

        public object AsObject
        {
            get
            {
                if (Raw == null)
                    return null;

                if (Property.TypeInfo.IsEnum)
                {
                    var enumValue =
                        (Enum)Enum.Parse(Property.TypeInfo.EnumType, AsString);
                    if (enumValue == null)
                        return AsString;

                    return enumValue;
                }
                if (Property.TypeInfo.IsNullable)
                    return Convert.ChangeType(Raw, Property.TypeInfo.UnderlyingType);
                if (Property.TypeInfo.IsFile)
                    return null;

                return Convert.ChangeType(Raw, Property.TypeInfo.Type);
            }
        }

        public CellValue()
        {
        }

        public CellValue(Property property)
        {
            Property = property;
        }
    }
}
