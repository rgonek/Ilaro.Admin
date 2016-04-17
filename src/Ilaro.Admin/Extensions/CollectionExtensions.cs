using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ilaro.Admin.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Determines whether the collection is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type.</typeparam>
        /// <param name="enumerable">The enumerable, which may be null or empty.</param>
        /// <returns>
        ///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }
            /* If this is a list, use the Count property. 
             * The Count property is O(1) while IEnumerable.Count() is O(N). */
            var collection = enumerable as ICollection<T>;
            if (collection != null)
            {
                return collection.Count < 1;
            }
            return !enumerable.Any();
        }

        /// <summary>
        /// Determines whether the collection is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type.</typeparam>
        /// <param name="collection">The collection, which may be null or empty.</param>
        /// <returns>
        ///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            if (collection == null)
            {
                return true;
            }
            return collection.Count < 1;
        }

        public static IDictionary<string, object> Merge(
            this IDictionary<string, object> dictionary1,
            object htmlAttributes)
        {
            return dictionary1.Merge(
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static IDictionary<string, object> Merge(
            this IDictionary<string, object> dictionary1,
            IDictionary<string, object> dictionary2)
        {
            if (dictionary1 == null)
            {
                dictionary1 = new Dictionary<string, object>();
            }
            if (dictionary2 == null)
            {
                dictionary2 = new Dictionary<string, object>();
            }

            return dictionary1.Union(dictionary2)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public static RouteValueDictionary ToRouteValueDictionary(this NameValueCollection collection)
        {
            var routeValueDictionary = new RouteValueDictionary();
            foreach (var key in collection.AllKeys)
            {
                routeValueDictionary.Add(key, collection[key]);
            }
            return routeValueDictionary;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
            this IEnumerable<TSource> source, 
            Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> knownKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}