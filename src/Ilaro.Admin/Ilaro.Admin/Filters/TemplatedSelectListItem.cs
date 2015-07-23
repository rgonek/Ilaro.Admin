using System.Web.Mvc;

namespace Ilaro.Admin.Filters
{
    public class TemplatedSelectListItem : SelectListItem
    {
        public string Template { get; set; }

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
            Template = template;
        }
    }
}