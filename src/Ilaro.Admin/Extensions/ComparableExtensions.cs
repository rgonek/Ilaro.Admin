using System;
using System.Linq;

namespace Ilaro.Admin.Extensions
{
    public static class ComparableExtensions
    {
        public static bool Between<T>(this T actual, T lower, T upper)
            where T : IComparable<T>
        {
            return actual.CompareTo(lower) >= 0 && actual.CompareTo(upper) < 0;
        }

        public static bool In<T>(this T source, params T[] list)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));
            return list.Contains(source);
        }
    }
}