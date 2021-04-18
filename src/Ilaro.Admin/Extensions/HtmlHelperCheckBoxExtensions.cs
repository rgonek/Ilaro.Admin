using System.Collections.Generic;
using Ilaro.Admin.Core;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Ilaro.Admin.Extensions
{
    /// <summary>
    /// Own CheckBox extensions, thanks that we create own metadata
    /// and based on we get unobtrusive validation attibute
    /// and pass this attributes to mvc CheckBox
    /// </summary>
    public static class HtmlHelperCheckBoxExtensions
    {
        public static IHtmlContent CheckBox(
            this IHtmlHelper htmlHelper,
            string name,
            bool isChecked,
            Property property)
        {
            return CheckBox(
                htmlHelper,
                name,
                isChecked,
                property,
                htmlAttributes: null);
        }

        public static IHtmlContent CheckBox(
            this IHtmlHelper htmlHelper,
            string name,
            bool isChecked,
            Property property,
            object htmlAttributes)
        {
            return CheckBox(
                htmlHelper,
                name,
                isChecked,
                property,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static IHtmlContent CheckBox(
            this IHtmlHelper htmlHelper,
            string name,
            bool isChecked,
            Property property,
            IDictionary<string, object> htmlAttributes)
        {
            //var validationAttributes = PropertyUnobtrusiveValidationAttributesGenerator
            //    .GetValidationAttributes(property, htmlHelper.ViewContext);

            //htmlAttributes = htmlAttributes.Merge(validationAttributes);

            return htmlHelper.CheckBox(name, isChecked, htmlAttributes);
        }
    }
}