using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Ilaro.Admin.Core;

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
                htmlAttributes: (IDictionary<string, object>)null);
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
            // create own metadata based on PropertyViewModel
            var metadata = new ModelMetadata(
                ModelMetadataProviders.Current,
                property.Entity.Type,
                null,
                property.TypeInfo.Type,
                property.Name)
            {
                DisplayName = property.Display
            };
            var validationAttributes =
                htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata);

            htmlAttributes = htmlAttributes.Merge(validationAttributes);

            return htmlHelper.TextArea(name, value, htmlAttributes);
        }
    }
}