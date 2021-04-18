using Ilaro.Admin.Core.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace Ilaro.Admin.Core.Filters
{
    public class TemplatedSelectListItem : SelectListItem
    {
        public string Template { get; private set; }

        public TemplatedSelectListItem()
        {
        }

        public TemplatedSelectListItem(
            string text,
            string value,
            string currentValue,
            string template = null,
            params string[] additionalMatchValues)
        {
            Text = text;
            Value = value;
            Selected = Value == currentValue || (additionalMatchValues != null && additionalMatchValues.Contains(currentValue));
            if (template.HasValue())
            {
                Template = "FilterTemplates/" + template;
            }
        }
    }
}