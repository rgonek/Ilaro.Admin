using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ilaro.Admin.Extensions
{
	public static class CollectionExtensions
	{
		/// <summary>
		/// Determines whether the collection is null or empty.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <returns>
		///   <c>true</c> if the collection is null or empty; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsNullOrEmpty(this ICollection collection)
		{
			if (collection != null)
			{
				return (collection.Count == 0);
			}
			return true;
		}

		/// <summary>
		/// Determines whether the collection is null or empty.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <returns>
		///   <c>true</c> if the collection is null or empty; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
		{
			if (collection != null)
			{
				return (collection.Count == 0);
			}
			return true;
		}

		public static IDictionary<string, object> Merge(this IDictionary<string, object> dictionary1, object htmlAttributes)
		{
			return dictionary1.Merge(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}

		public static IDictionary<string, object> Merge(this IDictionary<string, object> dictionary1, IDictionary<string, object> dictionary2)
		{
			return dictionary1.Union(dictionary2).ToDictionary(x => x.Key, x => x.Value);
		}
	}
}