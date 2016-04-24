using System;
using System.Linq;
using Ilaro.Admin.Extensions;
using SystemDataType = System.ComponentModel.DataAnnotations.DataType;

namespace Ilaro.Admin.Core
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
        public DataType DataType { get; internal set; }
        public SystemDataType? SourceDataType { get; internal set; }
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
            get { return Nullable.GetUnderlyingType(OriginalType) != null; }
        }

        public bool IsString
        {
            get { return OriginalType == typeof(string); }
        }

        public bool IsFileStoredInDb
        {
            get { return IsFile && IsString == false; }
        }

        public bool IsImage
        {
            get { return DataType == DataType.Image; }
        }

        public bool IsFile
        {
            get { return DataType == DataType.Image || DataType == DataType.File; }
        }

        public PropertyTypeInfo(Type type)
        {
            OriginalType = type;
            DeterminePropertyInfo();
            SetDataType();
        }

        public Type GetPropertyType()
        {
            return DataType == DataType.Enum ?
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

        private void SetDataType()
        {
            if (OriginalType.IsEnum)
            {
                DataType = DataType.Enum;
                EnumType = OriginalType;
            }
            else if (TypeInfo.IsNumber(OriginalType))
            {
                DataType = DataType.Numeric;
            }
            else if (OriginalType.In(typeof(DateTime), typeof(DateTime?)))
            {
                DataType = DataType.DateTime;
            }
            else if (OriginalType.In(typeof(bool), typeof(bool?)))
            {
                DataType = DataType.Bool;
            }
            else if (OriginalType == typeof(byte[]))
            {
                DataType = DataType.File;
            }
            else
            {
                DataType = DataType.Text;
            }
        }
    }
}