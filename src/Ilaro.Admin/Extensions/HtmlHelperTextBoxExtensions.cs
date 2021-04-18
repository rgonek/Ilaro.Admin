using System.Collections.Generic;
using Ilaro.Admin.Core;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Ilaro.Admin.Extensions
{
    /// <summary>
    /// Own TextBox extensions, thanks that we create own metadata and
    /// based on we get unobtrusive validation attibute and pass
    /// this attributes to mvc TextBox
    /// </summary>
    public static class HtmlHelperTextBoxExtensions
    {
        public static IHtmlContent TextBox(
            this IHtmlHelper htmlHelper,
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

        public static IHtmlContent TextBox(
            this IHtmlHelper htmlHelper,
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

        public static IHtmlContent TextBox(
            this IHtmlHelper htmlHelper,
            string name,
            object value,
            Property property,
            IDictionary<string, object> htmlAttributes)
        {
            //var validationAttributes = PropertyUnobtrusiveValidationAttributesGenerator
            //    .GetValidationAttributes(property, htmlHelper.ViewContext);

            //htmlAttributes = htmlAttributes.Merge(validationAttributes);

            return htmlHelper.TextBox(name, value, htmlAttributes);
        }
    }
}