using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Ilaro.Admin.Core;

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
                htmlAttributes: (IDictionary<string, object>)null);
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
            // create own metadata based on PropertyViewModel
            var metadata = new ModelMetadata(
                ModelMetadataProviders.Current,
                property.Entity.Type,
                null,
                property.TypeInfo.Type,
                property.Name);
            var validationAttributes =
                htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata);

            htmlAttributes = htmlAttributes.Merge(validationAttributes);

            return htmlHelper.CheckBox(name, isChecked, htmlAttributes);
        }
    }
}