using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Ilaro.Admin.Core;
using Ilaro.Admin.Validation;

namespace Ilaro.Admin.Extensions
{
    /// <summary>
    /// Own TextArea extensions, thanks that we create own metadata 
    /// and based on we get unobtrusive validation attibute 
    /// and pass this attributes to mvc TextArea
    /// </summary>
    public static class HtmlHelpersTextAreaExtensions
    {
        public static MvcHtmlString TextArea(
            this HtmlHelper htmlHelper,
            string name,
            string value,
            Property property)
        {
            return TextArea(
                htmlHelper,
                name,
                value,
                property,
                htmlAttributes: null);
        }

        public static MvcHtmlString TextArea(
            this HtmlHelper htmlHelper,
            string name,
            string value,
            Property property,
            object htmlAttributes)
        {
            return TextArea(
                htmlHelper,
                name,
                value,
                property,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString TextArea(
            this HtmlHelper htmlHelper,
            string name,
            string value,
            Property property,
            IDictionary<string, object> htmlAttributes)
        {
            var validationAttributes = PropertyUnobtrusiveValidationAttributesGenerator
                .GetValidationAttributes(property, htmlHelper.ViewContext);

            htmlAttributes = htmlAttributes.Merge(validationAttributes);

            return htmlHelper.TextArea(name, value, htmlAttributes);
        }
    }
}