using System;
using System.Collections.Generic;
using System.IO;
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
            IEntityFilter currentFilter,
            SelectListItem option,
            IEnumerable<IEntityFilter> filters,
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

        public static MvcHtmlString GetFilterIcon(
            this HtmlHelper htmlHelper,
            IEntityFilter filter)
        {
            if (filter is BoolEntityFilter)
            {
                return MvcHtmlString.Create("<i class=\"icon-check\"></i>");
            }
            if (filter is EnumEntityFilter)
            {
                return MvcHtmlString.Create("<i class=\"icon-list\"></i>");
            }
            if (filter is DateTimeEntityFilter)
            {
                return MvcHtmlString.Create("<i class=\"icon-calendar\"></i>");
            }

            return null;
        }

        public static MvcHtmlString SortColumnLink(
            this HtmlHelper htmlHelper,
            Entity entity,
            Column column,
            IEnumerable<IEntityFilter> filters,
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
                //routeValues.Add("od", "desc");
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

        public static MvcHtmlString Image(
            this HtmlHelper htmlHelper,
            CellValue value)
        {
            if (value.AsString.IsNullOrEmpty())
            {
                return null;
            }

            if (value.Property.TypeInfo.IsString)
            {
                // I have a path to image so easy to display
                var minSettings = value.Property.ImageOptions.Settings
                    .FirstOrDefault(x => x.IsMiniature) ??
                    value.Property.ImageOptions.Settings.FirstOrDefault();
                var bigSettings = value.Property.ImageOptions.Settings
                    .FirstOrDefault(x => x.IsBig) ??
                    value.Property.ImageOptions.Settings.FirstOrDefault();
                var minPath = Path.Combine(minSettings.SubPath, value.AsString)
                    .TrimStart('/');
                var bigPath = Path.Combine(bigSettings.SubPath, value.AsString)
                    .TrimStart('/');

                return MvcHtmlString.Create(
                    "<a href=\"/{1}\" class=\"open-modal\"><img src=\"/{0}\" class=\"img-polaroid\" /></a>"
                    .Fill(minPath, bigPath));
            }
            // I have a byte array so I must convert it to base64
            var base64 = Convert.ToBase64String((byte[])value.Raw);

            return MvcHtmlString.Create("<img src=\"data:image/jpg;base64,{0}\" class=\"img-polaroid\" />"
                .Fill(base64));
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