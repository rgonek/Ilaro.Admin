using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Ilaro.Admin.Core;
using Ilaro.Admin.Filters;
using Ilaro.Admin.Models;

namespace Ilaro.Admin.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString FilterOptionLink(
            this HtmlHelper htmlHelper,
            Entity entity,
            BaseFilter currentFilter,
            SelectListItem option,
            IEnumerable<BaseFilter> filters,
            string searchQuery,
            string order,
            string orderDirection,
            int perPage)
        {
            var routeValues = new Dictionary<string, object>
            {
                { "area", "IlaroAdmin" }, 
                { "entityName", entity.Name }, 
                { "pp", perPage }
            };
            if (!searchQuery.IsNullOrEmpty())
            {
                routeValues.Add("sq", searchQuery);
            }
            if (!order.IsNullOrEmpty() && !orderDirection.IsNullOrEmpty())
            {
                routeValues.Add("o", order);
                routeValues.Add("od", orderDirection);
            }
            var activeFilters = filters.Where(x => !x.Value.IsNullOrEmpty()).ToList();
            activeFilters.Remove(currentFilter);
            foreach (var filter in activeFilters)
            {
                routeValues.Add(filter.Property.Name, filter.Value);
            }

            if (!option.Value.IsNullOrEmpty())
            {
                routeValues.Add(currentFilter.Property.Name, option.Value);
            }

            return htmlHelper.ActionLink(
                option.Text,
                "Index",
                "Entities",
                new RouteValueDictionary(routeValues),
                null);
        }

        public static MvcHtmlString FilterOptionLink(
            this HtmlHelper htmlHelper,
            string text,
            string value,
            BaseFilter filter,
            object htmlAttributes = null)
        {
            var currentRouteValues =
                htmlHelper.ViewContext.RequestContext.HttpContext.Request.QueryString.ToRouteValueDictionary();
            var newRouteValues = new Dictionary<string, object>
            {
                { "area", "IlaroAdmin" }, 
                { "page", "1" }, 
                { filter.Property.Name, value }, 
            };

            return htmlHelper.ActionLink(
                text,
                "Index",
                "Entities",
                Merge(currentRouteValues, new RouteValueDictionary(newRouteValues)),
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        private static RouteValueDictionary Merge(
            this RouteValueDictionary original, 
            RouteValueDictionary extendedValues)
        {
            var merged = new RouteValueDictionary(original);

            foreach (var pairValue in extendedValues)
            {
                merged[pairValue.Key] = pairValue.Value;
            }

            return merged;
        }

        public static MvcHtmlString SortColumnLink(
            this HtmlHelper htmlHelper,
            Entity entity,
            Column column,
            IEnumerable<BaseFilter> filters,
            string searchQuery,
            int perPage)
        {
            var routeValues = new Dictionary<string, object>
            {
                { "area", "IlaroAdmin" }, 
                { "entityName", entity.Name }, 
                { "pp", perPage }
            };
            if (!searchQuery.IsNullOrEmpty())
            {
                routeValues.Add("sq", searchQuery);
            }

            routeValues.Add("o", column.Name);
            if (column.SortDirection == "up")
            {
                routeValues.Add("od", "desc");
            }
            else if (column.SortDirection == "down")
            {
                routeValues.Remove("o");
            }
            else
            {
                routeValues.Add("od", "asc");
            }

            var activeFilters = filters
                .Where(x => !x.Value.IsNullOrEmpty())
                .ToList();
            foreach (var filter in activeFilters)
            {
                routeValues.Add(filter.Property.Name, filter.Value);
            }

            return htmlHelper.ActionLink(
                column.DisplayName,
                "Index",
                "Entities",
                new RouteValueDictionary(routeValues),
                null);
        }

        /// <summary>
        /// It should by used only for small strings without html, 
        /// there is some html it should be used normal if condition
        /// </summary>
        public static MvcHtmlString Condition(
            this HtmlHelper htmlHelper,
            bool condition,
            string trueResult,
            string falseResult = null)
        {
            return MvcHtmlString.Create(condition ? trueResult : falseResult);
        }

        /// <summary>
        /// It should by used only for small strings without html, 
        /// there is some html it should be used normal if condition
        /// </summary>
        public static MvcHtmlString Condition(
            this HtmlHelper htmlHelper,
            bool condition,
            Func<string> trueResult,
            Func<string> falseResult)
        {
            return MvcHtmlString.Create(condition ? trueResult() : falseResult());
        }

        /// <summary>
        /// It should by used only for small strings without html, 
        /// there is some html it should be used normal if condition
        /// </summary>
        public static MvcHtmlString Condition(
            this HtmlHelper htmlHelper,
            bool condition,
            Func<string> trueResult)
        {
            return MvcHtmlString.Create(condition ? trueResult() : String.Empty);
        }

        /// <summary>
        /// Clear html field prefix
        /// </summary>
        /// <param name="htmlHelper"></param>
        public static void ClearPrefix(this HtmlHelper htmlHelper)
        {
            htmlHelper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "";
        }
    }
}