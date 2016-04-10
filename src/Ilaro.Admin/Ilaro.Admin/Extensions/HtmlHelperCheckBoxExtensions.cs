using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Ilaro.Admin.Core;
using Ilaro.Admin.Validation;

namespace Ilaro.Admin.Extensions
{
    /// <summary>
    /// Own CheckBox extensions, thanks that we create own metadata 
    /// and based on we get unobtrusive validation attibute 
    /// and pass this attributes to mvc CheckBox
    /// </summary>
    public static class HtmlHelperCheckBoxExtensions
    {
        public static MvcHtmlString CheckBox(
            this HtmlHelper htmlHelper,
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

        public static MvcHtmlString CheckBox(
            this HtmlHelper htmlHelper,
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

        public static MvcHtmlString CheckBox(
            this HtmlHelper htmlHelper,
            string name,
            bool isChecked,
            Property property,
            IDictionary<string, object> htmlAttributes)
        {
            var validationAttributes = PropertyUnobtrusiveValidationAttributesGenerator
                .GetValidationAttributes(property, htmlHelper.ViewContext);

            htmlAttributes = htmlAttributes.Merge(validationAttributes);

            return htmlHelper.CheckBox(name, isChecked, htmlAttributes);
        }
    }
}