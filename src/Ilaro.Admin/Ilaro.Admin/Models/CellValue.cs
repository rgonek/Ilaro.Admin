using System;
using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Models
{
    public class CellValue
    {
        public string AsString { get; set; }

        public object Raw { get; set; }

        public Property Property { get; set; }

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
                if (Property.TypeInfo.DataType == DataType.Enum)
                {
                    var enumValue = 
                        (Enum)Enum.Parse(Property.TypeInfo.EnumType, AsString);
                    if (enumValue == null)
                    {
                        return AsString;
                    }

                    // TODO: localization
                    //return 
                    //    enumValue.GetDescription() ?? 
                    //    enumValue.ToString().SplitCamelCase();
                    return enumValue.ToString().SplitCamelCase();
                }

                return Convert.ChangeType(AsString, Property.TypeInfo.Type);
            }
        }
    }
}
