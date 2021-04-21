using System;
using System.Linq;
using System.Collections.Generic;
using Ilaro.Admin.Core;
using Microsoft.AspNetCore.Html;
using System.Globalization;
using Ilaro.Admin.Core.Extensions;
using Ilaro.Admin.Core.Filters;
using Ilaro.Admin.Core.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ilaro.Admin.Extensions
{
    public static class HtmlHelperExtensions
    {
        //public static IHtmlContent FilterOptionLink(
        //    this HtmlHelper htmlHelper,
        //    string text,
        //    string value,
        //    BaseFilter filter,
        //    object htmlAttributes = null)
        //{
        //    var currentRouteValues = htmlHelper.ViewContext.HttpContext.Request.Query.ToRouteValueDictionary();
        //    var newRouteValues = new Dictionary<string, object>
        //    {
        //        { "area", "IlaroAdmin" },
        //        { "page", "1" },
        //        { filter.Property.Name, value }
        //    };
        //    var attr = Merge(currentRouteValues, new RouteValueDictionary(newRouteValues));

        //    return htmlHelper.ActionLink(
        //        text,
        //        htmlHelper.ViewContext.RouteData.Values["action"].ToStringSafe() ?? "Index",
        //        "Entities",
        //        Merge(currentRouteValues, new RouteValueDictionary(newRouteValues)),
        //        HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        //}

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

        public static IHtmlContent SortColumnLink(
            this IHtmlHelper htmlHelper,
            Entity entity,
            Column column,
            IEnumerable<BaseFilter> filters,
            string searchQuery,
            int perPage)
        {
            var entityName = entity == null ?
                null :
                entity.Name;
            var routeValues = new Dictionary<string, object>
            {
                { "area", "IlaroAdmin" },
                { "EntityName", entityName },
                { "pp", perPage }
            };
            if (!searchQuery.IsNullOrEmpty())
            {
                routeValues.Add("sq", searchQuery);
            }

            routeValues.Add("o", column.Name);
            if (column.SortDirection == "up")
            {
                routeValues["od"] = "desc";
            }
            else if (column.SortDirection == "down")
            {
                routeValues["od"] = "asc";
            }

            var activeFilters = filters
                .Where(x => x.DisplayInUI && !x.Value.IsNullOrEmpty())
                .ToList();
            foreach (var filter in activeFilters)
            {
                routeValues[filter.Property.Name] = filter.Value;
            }

            return htmlHelper.ActionLink(
                column.DisplayName,
                htmlHelper.ViewContext.RouteData.Values["action"].ToStringSafe() ?? "Index",
                "Entities",
                new RouteValueDictionary(routeValues),
                null);
        }

        public static IHtmlContent ConcurrencyCheck(
            this IHtmlHelper htmlHelper,
            object concurrencyCheckValue)
        {
            if (concurrencyCheckValue == null)
                return HtmlString.Empty;

            if (concurrencyCheckValue is DateTime)
            {
                concurrencyCheckValue = (concurrencyCheckValue as DateTime?).Value
                    .ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.CurrentCulture);
            }

            return htmlHelper.Hidden("__ConcurrencyCheck", concurrencyCheckValue);
        }

        //public static string PageUrl(
        //    this UrlHelper urlHelper,
        //    int pageNumber)
        //{
        //    var routeDataValues = urlHelper.ActionContext.RouteData.Values
        //        .Merge(urlHelper.ActionContext.HttpContext.Request.QueryString.ToRouteValueDictionary());

        //    SetPageNumber(routeDataValues, pageNumber);

        //    var actionName = routeDataValues["action"].ToStringSafe();

        //    return urlHelper.Action(actionName, routeDataValues);
        //}

        private static void SetPageNumber(
            RouteValueDictionary routeDataValues,
            int pageNumber)
        {
            // Avoid canonical errors when pageNumber is equal to 1.
            if (pageNumber == 1)
            {
                if (routeDataValues.ContainsKey("page"))
                {
                    routeDataValues.Remove("page");
                }
            }
            else
            {
                routeDataValues["page"] = pageNumber;
            }
        }
    }
}