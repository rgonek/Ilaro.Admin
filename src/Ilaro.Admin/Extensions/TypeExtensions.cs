using System;
using System.Reflection;

namespace Ilaro.Admin.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsDelegate(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return type.GetTypeInfo().IsSubclassOf(typeof(Delegate));
        }

        public static bool TypeIsPublicClass(this Type type)
        {
            return (type != null && type.IsPublic && type.IsClass && !type.IsAbstract);
        }

        /// <summary>
        /// Returns true if this type is in the <paramref name="namespace"/> namespace
        /// or one of its sub-namespaces.
        /// </summary>
        /// <param name="this">The type to test.</param>
        /// <param name="namespace">The namespace to test.</param>
        /// <returns>True if this type is in the <paramref name="namespace"/> namespace
        /// or one of its sub-namespaces; otherwise, false.</returns>
        public static bool IsInNamespace(this Type @this, string @namespace)
        {
            if (@this == null) throw new ArgumentNullException(nameof(@this));
            if (@namespace == null) throw new ArgumentNullException(nameof(@namespace));

            return @this.Namespace != null &&
                (@this.Namespace == @namespace || @this.Namespace.StartsWith(@namespace + ".", StringComparison.Ordinal));
        }

        /// <summary>
        /// Returns true if this type is in the same namespace as <typeparamref name="T"/>
        /// or one of its sub-namespaces.
        /// </summary>
        /// <param name="this">The type to test.</param>
        /// <returns>True if this type is in the same namespace as <typeparamref name="T"/>
        /// or one of its sub-namespaces; otherwise, false.</returns>
        public static bool IsInNamespaceOf<T>(this Type @this)
        {
            if (@this == null) throw new ArgumentNullException(nameof(@this));

            return IsInNamespace(@this, typeof(T).Namespace);
        }

        /// <summary>
        /// Determines whether this type is assignable to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to test assignability to.</typeparam>
        /// <returns>True if this type is assignable to references of type
        /// <typeparamref name="T"/>; otherwise, False.</returns>
        public static bool IsAssignableTo<T>(this Type @this)
        {
            if (@this == null) throw new ArgumentNullException(nameof(@this));

            return typeof(T).GetTypeInfo().IsAssignableFrom(@this.GetTypeInfo());
        }
    }
}