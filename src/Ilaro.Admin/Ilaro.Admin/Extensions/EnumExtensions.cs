using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Extensions
{
    public static class EnumExtensions
    {
        public static SelectList GetSelectList(this CascadeOption deleteOption)
        {
            var options = deleteOption.GetOptions();
            options.Remove(((int)CascadeOption.AskUser).ToString());

            return new SelectList(options, "Key", "Value", (int)deleteOption);
        }

        public static IDictionary<string, string> GetOptions(
            this Enum enumObject,
            string key = "",
            string value = "")
        {
            return enumObject.GetType().GetOptions(key, value);
        }

        public static IDictionary<string, string> GetOptions(
            this Type type,
            string key = "",
            string value = "")
        {
            if (!type.IsEnum)
            {
                return null;
            }

            var dict = new Dictionary<string, string>();

            if (!key.IsNullOrEmpty() || !value.IsNullOrEmpty())
            {
                dict.Add(key, value);
            }

            foreach (Enum item in Enum.GetValues(type))
            {
                // TODO: Localize Enums
                //dict.Add(
                //    Convert.ToInt32(item).ToString(), 
                //    item.GetDescription() ?? item.ToString().SplitCamelCase());
                dict.Add(
                    Convert.ToInt32(item).ToString(),
                    item.ToString().SplitCamelCase());
            }

            return dict;
        }
    }
}