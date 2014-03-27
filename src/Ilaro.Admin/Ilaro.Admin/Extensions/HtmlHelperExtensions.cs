using Ilaro.Admin.EntitiesFilters;
using Ilaro.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ilaro.Admin.Extensions;
using System.Web.Mvc.Html;

namespace Ilaro.Admin.Extensions
{
	public static class HtmlHelperExtensions
	{
		public static MvcHtmlString FilterOptionLink(this HtmlHelper htmlHelper, EntityViewModel entity, IEntityFilter currentFilter, SelectListItem option, IList<IEntityFilter> filters, string searchQuery, string order, string orderDirection, int perPage)
		{
			var routeValues = new Dictionary<string, object> { { "entityName", entity.Name }, { "pp", perPage } };
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

			return htmlHelper.ActionLink(option.Text, "Details", new RouteValueDictionary(routeValues));
		}

		public static MvcHtmlString GetFilterIcon(this HtmlHelper htmlHelper, IEntityFilter filter)
		{
			if (filter is BoolEntityFilter)
			{
				return MvcHtmlString.Create("<i class=\"icon-check\"></i>");
			}
			else if (filter is EnumEntityFilter)
			{
				return MvcHtmlString.Create("<i class=\"icon-list\"></i>");
			}
			else if (filter is DateTimeEntityFilter)
			{
				return MvcHtmlString.Create("<i class=\"icon-calendar\"></i>");
			}

			return null;
		}

		public static MvcHtmlString SortColumnLink(this HtmlHelper htmlHelper, EntityViewModel entity, ColumnViewModel column, IList<IEntityFilter> filters, string searchQuery, int perPage)
		{
			var routeValues = new Dictionary<string, object> { { "entityName", entity.Name }, { "pp", perPage } };
			if (!searchQuery.IsNullOrEmpty())
			{
				routeValues.Add("sq", searchQuery);
			}

			routeValues.Add("o", column.Name);
			if (column.SortDirection == "up")
			{
				routeValues.Add("od", "desc");
			}
			else
			{
				routeValues.Add("od", "asc");
			}

			var activeFilters = filters.Where(x => !x.Value.IsNullOrEmpty()).ToList();
			foreach (var filter in activeFilters)
			{
				routeValues.Add(filter.Property.Name, filter.Value);
			}
			
			return htmlHelper.ActionLink(column.DisplayName, "Details", new RouteValueDictionary(routeValues));
		}

		public static MvcHtmlString Image(this HtmlHelper htmlHelper, CellValueViewModel cell)
		{
			if (cell.Value.IsNullOrEmpty())
			{
				return null;
			}

			var minSettings = cell.Property.ImageOptions.Settings.FirstOrDefault(x => x.IsMiniature) ?? cell.Property.ImageOptions.Settings.FirstOrDefault();
			var bigSettings = cell.Property.ImageOptions.Settings.FirstOrDefault(x => x.IsBig) ?? cell.Property.ImageOptions.Settings.FirstOrDefault();
			var minPath = Path.Combine(minSettings.SubPath, cell.Value).TrimStart('/');
			var bigPath = Path.Combine(bigSettings.SubPath, cell.Value).TrimStart('/');

			return MvcHtmlString.Create(string.Format("<a href=\"/{1}\" class=\"open-modal\"><img src=\"/{0}\" class=\"img-polaroid\" /></a>", minPath, bigPath));
		}

		/// <summary>
		/// It should by used only for small strings without html, there is some html it should be used normal if condition
		/// </summary>
		public static MvcHtmlString Condition(this HtmlHelper htmlHelper, bool condition, string trueResult, string falseResult = null)
		{
			return MvcHtmlString.Create(condition ? trueResult : falseResult);
		}

		/// <summary>
		/// It should by used only for small strings without html, there is some html it should be used normal if condition
		/// </summary>
		public static MvcHtmlString Condition(this HtmlHelper htmlHelper, bool condition, Func<string> trueResult, Func<string> falseResult)
		{
			return MvcHtmlString.Create(condition ? trueResult() : falseResult());
		}

		/// <summary>
		/// It should by used only for small strings without html, there is some html it should be used normal if condition
		/// </summary>
		public static MvcHtmlString Condition(this HtmlHelper htmlHelper, bool condition, Func<string> trueResult)
		{
			return MvcHtmlString.Create(condition ? trueResult() : String.Empty);
		}
	}
}