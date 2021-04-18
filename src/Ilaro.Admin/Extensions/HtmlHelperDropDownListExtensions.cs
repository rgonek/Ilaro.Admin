using System.Collections.Generic;
using Ilaro.Admin.Core;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Ilaro.Admin.Extensions
{
    /// <summary>
    /// Own DropDownList extensions, thanks that we create own metadata
    /// and based on we get unobtrusive validation attibute
    /// and pass this attributes to mvc DropDownList
    /// </summary>
    public static class HtmlHelperDropDownListExtensions
    {
        public static IHtmlContent DropDownList(
            this IHtmlHelper htmlHelper,
            string name,
            IEnumerable<SelectListItem> selectList,
            Property property)
        {
            return DropDownList(
                htmlHelper,
                name,
                selectList,
                property,
                htmlAttributes: null);
        }

        public static IHtmlContent DropDownList(
            this IHtmlHelper htmlHelper,
            string name,
            IEnumerable<SelectListItem> selectList,
            Property property,
            object htmlAttributes)
        {
            return DropDownList(
                htmlHelper,
                name,
                selectList,
                property,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static IHtmlContent DropDownList(
            this IHtmlHelper htmlHelper,
            string name,
            IEnumerable<SelectListItem> selectList,
            Property property,
            IDictionary<string, object> htmlAttributes)
        {
            //var validationAttributes = PropertyUnobtrusiveValidationAttributesGenerator
            //    .GetValidationAttributes(property, htmlHelper.ViewContext);

            //htmlAttributes = htmlAttributes.Merge(validationAttributes);

            return htmlHelper.DropDownList(name, selectList, htmlAttributes);
        }
    }
}