using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Ilaro.Admin.Configuration
{
    internal static class TypeExtensions
    {
        private const BindingFlags PropertiesOfClassHierarchy = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        internal static IEnumerable<MemberInfo> DecodeMemberAccessExpression<TEntity>(params Expression<Func<TEntity, object>>[] expressions)
        {
            return expressions.Select(x => DecodeMemberAccessExpression<TEntity, object>(x));
        }

        internal static MemberInfo DecodeMemberAccessExpression<TEntity>(Expression<Func<TEntity, object>> expression)
        {
            return DecodeMemberAccessExpression<TEntity, object>(expression);
        }

        internal static MemberInfo DecodeMemberAccessExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            if (expression.Body.NodeType != ExpressionType.MemberAccess)
            {
                if ((expression.Body.NodeType == ExpressionType.Convert) && (expression.Body.Type == typeof(TProperty)))
                {
                    return ((MemberExpression)((UnaryExpression)expression.Body).Operand).Member;
                }
                throw new Exception(string.Format("Invalid expression type: Expected ExpressionType.MemberAccess, Found {0}",
                    expression.Body.NodeType));
            }
            return ((MemberExpression)expression.Body).Member;
        }

        /// <summary>
        /// Decode a member access expression of a specific ReflectedType
        /// </summary>
        /// <typeparam name="TEntity">Type to reflect</typeparam>
        /// <param name="expression">The expression of the property getter</param>
        /// <returns>The <see cref="MemberInfo"/> os the ReflectedType. </returns>
        internal static IEnumerable<MemberInfo> DecodeMemberAccessExpressionOf<TEntity>(params Expression<Func<TEntity, object>>[] expressions)
        {
            return expressions.Select(x => DecodeMemberAccessExpressionOf(x));
        }

        /// <summary>
        /// Decode a member access expression of a specific ReflectedType
        /// </summary>
        /// <typeparam name="TEntity">Type to reflect</typeparam>
        /// <typeparam name="TProperty">Type of property</typeparam>
        /// <param name="expression">The expression of the property getter</param>
        /// <returns>The <see cref="MemberInfo"/> os the ReflectedType. </returns>
        internal static MemberInfo DecodeMemberAccessExpressionOf<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            var memberOfDeclaringType = DecodeMemberAccessExpression(expression);
            if (typeof(TEntity).IsInterface)
            {
                // Type.GetProperty(string name,Type returnType) does not work properly with interfaces
                return memberOfDeclaringType;
            }

            var propertyInfo = memberOfDeclaringType as PropertyInfo;
            if (propertyInfo != null)
            {
                return typeof(TEntity).GetProperty(propertyInfo.Name, PropertiesOfClassHierarchy, null, propertyInfo.PropertyType, new System.Type[0], null);
            }
            if (memberOfDeclaringType is FieldInfo)
            {
                return typeof(TEntity).GetField(memberOfDeclaringType.Name, PropertiesOfClassHierarchy);
            }
            throw new NotSupportedException();
        }

        internal static T GetAttribute<T>(this object[] attributes)
        {
            return attributes
                .OfType<T>()
                .FirstOrDefault();
        }

        internal static IEnumerable<T> GetAttributes<T>(this object[] attributes)
        {
            return attributes
                .OfType<T>();
        }
    }
}