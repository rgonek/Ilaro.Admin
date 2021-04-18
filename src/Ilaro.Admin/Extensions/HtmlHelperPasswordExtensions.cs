using System.Collections.Generic;
using Ilaro.Admin.Core;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Ilaro.Admin.Extensions
{
    /// <summary>
    /// Own Password extensions, thanks that we create own metadata
    /// and based on we get unobtrusive validation attibute and pass
    /// this attributes to mvc Password
    /// </summary>
    public static class HtmlHelperPasswordExtensions
    {
        public static IHtmlContent Password(
            this IHtmlHelper htmlHelper,
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

        public static IHtmlContent Password(
            this IHtmlHelper htmlHelper,
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

        public static IHtmlContent Password(
            this IHtmlHelper htmlHelper,
            string name,
            object value,
            Property property,
            IDictionary<string, object> htmlAttributes)
        {
            //var validationAttributes = PropertyUnobtrusiveValidationAttributesGenerator
            //    .GetValidationAttributes(property, htmlHelper.ViewContext);

            //htmlAttributes = htmlAttributes.Merge(validationAttributes);

            return htmlHelper.Password(name, value, htmlAttributes);
        }
    }
}