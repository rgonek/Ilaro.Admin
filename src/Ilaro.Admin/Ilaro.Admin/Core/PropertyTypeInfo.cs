using System;
using System.Linq;
using Ilaro.Admin.Extensions;
using SystemDataType = System.ComponentModel.DataAnnotations.DataType;

namespace Ilaro.Admin.Core
{
    public class PropertyTypeInfo
    {
        public Type Type { get; internal set; }
        public Type NotNullableType
        {
            get
            {
                var notNullableType = UnderlyingType;
                if (notNullableType != null)
                    return notNullableType;
                return Type;
            }
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
            get { return TypeInfo.IsReal(Type); }
        }
        public bool IsFloatingPoint
        {
            get { return TypeInfo.IsFloatingPoint(Type); }
        }
        public bool IsNumber
        {
            get { return TypeInfo.IsNumber(Type); }
        }
        public bool IsBool
        {
            get { return TypeInfo.IsBool(Type); }
        }
        public bool IsAvailableForSearch
        {
            get { return TypeInfo.IsAvailableForSearch(Type); }
        }

        public bool IsEnum
        {
            get { return Type.IsEnum; }
        }

        public bool IsNullable
        {
            get { return Nullable.GetUnderlyingType(Type) != null; }
        }

        public Type UnderlyingType
        {
            get { return Nullable.GetUnderlyingType(Type); }
        }

        public bool IsString
        {
            get { return Type == typeof(string); }
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
            Type = type;
            DeterminePropertyInfo();
            SetDataType();
        }

        private void DeterminePropertyInfo()
        {
            // for example for string PropertyType.GetInterface("IEnumerable`1") 
            // is not null, so we must check if type has sub type 
            IsCollection =
                Type.GetInterface("IEnumerable`1") != null &&
                Type.GetGenericArguments().Any();
            if (IsCollection)
            {
                var subType = Type.GetGenericArguments().Single();
                Type = subType;
            }

            IsSystemType = Type.Namespace.StartsWith("System");
        }

        private void SetDataType()
        {
            if (Type.IsEnum)
            {
                DataType = DataType.Enum;
                EnumType = Type;
            }
            else if (TypeInfo.IsNumber(Type))
            {
                DataType = DataType.Numeric;
            }
            else if (Type.In(typeof(DateTime), typeof(DateTime?)))
            {
                DataType = DataType.DateTime;
            }
            else if (Type.In(typeof(bool), typeof(bool?)))
            {
                DataType = DataType.Bool;
            }
            else if (Type == typeof(byte[]))
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