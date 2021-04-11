using System;

namespace Ilaro.Admin.Core
{
    public static class TypeHelpers
    {
        public static bool TypeAllowsNullValue(Type type)
        {
            return (!type.IsValueType || IsNullableValueType(type));
        }

        public static bool IsNullableValueType(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }
    }
}