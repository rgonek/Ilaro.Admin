using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Ilaro.Admin.Core;
using Ilaro.Admin.Validation;

namespace Ilaro.Admin.Extensions
{
    /// <summary>
    /// Own TextBox extensions, thanks that we create own metadata and 
    /// based on we get unobtrusive validation attibute and pass 
    /// this attributes to mvc TextBox
    /// </summary>
    public static class HtmlHelperTextBoxExtensions
    {
        public static MvcHtmlString TextBox(
            this HtmlHelper htmlHelper,
            string name,
            object value,
            Property property)
        {
            return TextBox(
                htmlHelper,
                name,
                value,
                property,
                htmlAttributes: null);
        }

        public static MvcHtmlString TextBox(
            this HtmlHelper htmlHelper,
            string name,
            object value,
            Property property,
            object htmlAttributes)
        {
            return TextBox(
                htmlHelper,
                name,
                value,
                property,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString TextBox(
            this HtmlHelper htmlHelper,
            string name,
            object value,
            Property property,
            IDictionary<string, object> htmlAttributes)
        {
            var validationAttributes = PropertyUnobtrusiveValidationAttributesGenerator
                .GetValidationAttributes(property, htmlHelper.ViewContext);

            htmlAttributes = htmlAttributes.Merge(validationAttributes);

            return htmlHelper.TextBox(name, value, htmlAttributes);
        }
    }
}