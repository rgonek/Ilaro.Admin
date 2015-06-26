using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Ilaro.Admin.ViewModels;

namespace Ilaro.Admin.Extensions
{
    /// <summary>
    /// Own DropDownList extensions, thanks that we create own metadata 
    /// and based on we get unobtrusive validation attibute 
    /// and pass this attributes to mvc DropDownList
    /// </summary>
    public static class HtmlHelperDropDownListExtensions
    {
        public static MvcHtmlString DropDownList(
            this HtmlHelper htmlHelper,
            string name,
            IEnumerable<SelectListItem> selectList,
            Property property)
        {
            return DropDownList(
                htmlHelper,
                name,
                selectList,
                property,
                htmlAttributes: (IDictionary<string, object>)null);
        }

        public static MvcHtmlString DropDownList(
            this HtmlHelper htmlHelper,
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

        public static MvcHtmlString DropDownList(
            this HtmlHelper htmlHelper,
            string name,
            IEnumerable<SelectListItem> selectList,
            Property property,
            IDictionary<string, object> htmlAttributes)
        {
            // create own metadata based on PropertyViewModel
            var metadata = new ModelMetadata(
                ModelMetadataProviders.Current,
                property.Entity.Type,
                null,
                property.PropertyType,
                property.Name);
            var validationAttributes =
                htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata);

            htmlAttributes = htmlAttributes.Merge(validationAttributes);

            return htmlHelper.DropDownList(name, selectList, htmlAttributes);
        }
    }
}