using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.ViewModels
{
    public class CellValueViewModel
    {
        public string Value { get; set; }

        public object RawValue { get; set; }

        public Property Property { get; set; }

        public bool? BoolValue
        {
            get
            {
                if (Value.IsNullOrEmpty())
                {
                    return null;
                }

                return bool.Parse(Value.ToString());
            }
        }

        public object ObjectValue
        {
            get
            {
                if (Property.DataType == ViewModels.DataType.Enum)
                {
                    var enumValue = (Enum)Enum.Parse(Property.EnumType, Value);
                    if (enumValue == null)
                    {
                        return Value;
                    }

					//return enumValue.GetDescription() ?? enumValue.ToString().SplitCamelCase();
					return enumValue.ToString().SplitCamelCase();
                }

                return Convert.ChangeType(Value, Property.PropertyType);
            }
        }
    }
}