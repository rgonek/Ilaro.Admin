using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ilaro.Admin.DataAnnotations;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Core
{
    public class PropertyTypeInfo
    {
        public Type Type { get; internal set; }
        public DataType DataType { get; internal set; }
        public System.ComponentModel.DataAnnotations.DataType? SourceDataType { get; private set; }
        public Type EnumType { get; private set; }
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

        public bool IsString
        {
            get { return Type == typeof(string); }
        }

        public bool IsFileStoredInDb
        {
            get { return DataType == DataType.File && IsString == false; }
        }

        public PropertyTypeInfo(Type type, object[] attributes)
        {
            Type = type;
            DeterminePropertyInfo();
            SetDataType(attributes);
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

        private void SetDataType(object[] attributes)
        {
            var dataTypeAttribute =
                attributes.OfType<DataTypeAttribute>().FirstOrDefault();
            if (dataTypeAttribute != null)
            {
                SourceDataType = dataTypeAttribute.DataType;
                DataType = DataTypeConverter.Convert(dataTypeAttribute.DataType);

                return;
            }

            var enumDataTypeAttribute =
                attributes.OfType<EnumDataTypeAttribute>().FirstOrDefault();

            if (enumDataTypeAttribute != null)
            {
                DataType = DataType.Enum;
                EnumType = enumDataTypeAttribute.EnumType;
            }
            else if (Type.IsEnum)
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

            var imageAttribute =
                attributes.OfType<ImageAttribute>().FirstOrDefault();
            var imageSettingsAttributes =
                attributes.OfType<ImageSettingsAttribute>().ToList();

            if (imageAttribute == null &&
                !imageSettingsAttributes.Any() &&
                DataType != DataType.File)
                return;

            DataType = DataType.File;
        }
    }
}