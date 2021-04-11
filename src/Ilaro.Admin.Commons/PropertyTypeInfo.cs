using System;
using System.Linq;

namespace Ilaro.Admin.Common
{
    public class PropertyTypeInfo
    {
        public Type OriginalType { get; internal set; }
        public Type NotNullableType
        {
            get
            {
                var notNullableType = UnderlyingType;
                if (notNullableType != null)
                    return notNullableType;
                return OriginalType;
            }
        }
        public Type UnderlyingType
        {
            get { return Nullable.GetUnderlyingType(OriginalType); }
        }

        public Type EnumType { get; internal set; }

        /// <summary>
        /// If a property type (or sub type if property is a collection)
        /// is a system type (namespace starts with "System") or not.
        /// </summary>
        public bool IsSystemType { get; private set; }

        /// <summary>
        /// If property is a collection and has sub type.
        /// </summary>
        public bool IsCollection { get; private set; }

        public bool IsReal
        {
            get { return TypeInfo.IsReal(OriginalType); }
        }

        public bool IsFloatingPoint
        {
            get { return TypeInfo.IsFloatingPoint(OriginalType); }
        }

        public bool IsNumber
        {
            get { return TypeInfo.IsNumber(OriginalType); }
        }
        public bool IsBool
        {
            get { return TypeInfo.IsBool(OriginalType); }
        }
        public bool IsGuid
        {
            get { return TypeInfo.IsGuid(OriginalType); }
        }
        public bool IsAvailableForSearch
        {
            get { return TypeInfo.IsAvailableForSearch(OriginalType); }
        }

        public bool IsEnum
        {
            get { return OriginalType.IsEnum; }
        }

        public bool IsNullable
        {
            get { return TypeHelpers.IsNullableValueType(OriginalType); }
        }

        public bool IsString
        {
            get { return OriginalType == typeof(string); }
        }

        public PropertyTypeInfo(Type type)
        {
            OriginalType = type;
            DeterminePropertyInfo();
        }

        public Type GetPropertyType()
        {
            return IsEnum ?
                EnumType :
                NotNullableType;
        }

        private void DeterminePropertyInfo()
        {
            // for example for string PropertyType.GetInterface("IEnumerable`1")
            // is not null, so we must check if type has sub type
            IsCollection =
                OriginalType.GetInterface("IEnumerable`1") != null &&
                OriginalType.GetGenericArguments().Any();
            if (IsCollection)
            {
                var subType = OriginalType.GetGenericArguments().Single();
                OriginalType = subType;
            }

            IsSystemType = OriginalType.Namespace.StartsWith("System");
        }
    }
}
