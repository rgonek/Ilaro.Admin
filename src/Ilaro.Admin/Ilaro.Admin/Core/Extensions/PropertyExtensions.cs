using Ilaro.Admin.Extensions;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Ilaro.Admin.Core.Extensions
{
    public static class PropertyExtensions
    {

        public static MultiSelectList GetPossibleValues(this Property property, bool addChooseItem = true)
        {
            if (property.IsForeignKey)
            {
                var options = new Dictionary<string, string>();

                if (addChooseItem)
                {
                    options.Add(String.Empty, IlaroAdminResources.Choose);
                }
                options = options.Union(property.Value.PossibleValues).ToDictionary(x => x.Key, x => x.Value);

                return property.TypeInfo.IsCollection ?
                    new MultiSelectList(options, "Key", "Value", property.Value.Values) :
                    new SelectList(options, "Key", "Value", property.Value.AsString);
            }
            else
            {
                var options = addChooseItem ?
                    property.TypeInfo.EnumType.GetOptions(String.Empty, IlaroAdminResources.Choose) :
                    property.TypeInfo.EnumType.GetOptions();

                if (property.TypeInfo.IsEnum)
                {
                    return new SelectList(
                        options,
                        "Key",
                        "Value",
                        property.Value.AsObject);
                }

                return new SelectList(options, "Key", "Value", property.Value.AsString);
            }
        }
    }
}