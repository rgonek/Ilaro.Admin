using System.Web.Mvc;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Filters
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
            string template = null)
        {
            Text = text;
            Value = value;
            Selected = Value == currentValue;
            if (template.HasValue())
            {
                Template = "FilterTemplates/" + template;   
            }
        }
    }
}