using System;
using System.Reflection;

namespace Ilaro.Admin.Extensions
{
    static class TypeExtensions
    {
        public static bool IsDelegate(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return type.GetTypeInfo().IsSubclassOf(typeof(Delegate));
        }
    }
}