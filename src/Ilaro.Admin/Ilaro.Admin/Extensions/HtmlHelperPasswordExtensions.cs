using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Ilaro.Admin.Core;
using Ilaro.Admin.Validation;

namespace Ilaro.Admin.Extensions
{
    /// <summary>
    /// Own Password extensions, thanks that we create own metadata 
    /// and based on we get unobtrusive validation attibute and pass 
    /// this attributes to mvc Password
    /// </summary>
    public static class HtmlHelperPasswordExtensions
    {
        public static MvcHtmlString Password(
            this HtmlHelper htmlHelper,
            string name,
            object value,
            Property property)
        {
            return Password(
                htmlHelper,
                name, value,
                property,
                htmlAttributes: null);
        }

        public static MvcHtmlString Password(
            this HtmlHelper htmlHelper,
            string name,
            object value,
            Property property,
            object htmlAttributes)
        {
            return Password(
                htmlHelper,
                name,
                value,
                property,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString Password(
            this HtmlHelper htmlHelper,
            string name,
            object value,
            Property property,
            IDictionary<string, object> htmlAttributes)
        {
            var validationAttributes = PropertyUnobtrusiveValidationAttributesGenerator
                .GetValidationAttributes(property, htmlHelper.ViewContext);

            htmlAttributes = htmlAttributes.Merge(validationAttributes);

            return htmlHelper.Password(name, value, htmlAttributes);
        }
    }
}