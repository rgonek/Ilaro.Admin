using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Ilaro.Admin.DataAnnotations;
using Ilaro.Admin.Extensions;
using Resources;

namespace Ilaro.Admin.Core
{
    [DebuggerDisplay("Property {Name}")]
    public class Property
    {
        public Entity Entity { get; private set; }

        public PropertyInfo PropertyInfo { get; internal set; }

        public string Name { get; internal set; }

        private string _columnName;
        public string ColumnName
        {
            get { return _columnName; }
            internal set
            {
                if (value.StartsWith("[") && value.EndsWith("]"))
                    _columnName = value;
                else
                    _columnName = "[" + value + "]";
            }
        }

        public string DisplayName { get; internal set; }

        public string GroupName { get; internal set; }

        public string Description { get; internal set; }

        public string Prompt { get; internal set; }

        public string Format { get; internal set; }

        /// <summary>
        /// If property is a entity key.
        /// </summary>
        public bool IsKey { get; internal set; }

        public bool IsAutoKey
        {
            get { return IsKey && TypeInfo.DataType != DataType.Text; }
        }

        public bool IsVisible { get; internal set; }

        public bool IsSearchable { get; internal set; }

        /// <summary>
        /// Is property is a foreign key.
        /// </summary>
        public bool IsForeignKey { get; internal set; }

        public Entity ForeignEntity { get; internal set; }

        public string ForeignEntityName { get; internal set; }

        public Property ReferenceProperty { get; internal set; }

        public string ReferencePropertyName { get; internal set; }

        public bool IsRequired { get; internal set; }
        public string RequiredErrorMessage { get; internal set; }

        public DeleteOption DeleteOption { get; internal set; }
        public FileOptions FileOptions { get; internal set; } = new FileOptions();
        public PropertyTemplate Template { get; internal set; } = new PropertyTemplate();
        public PropertyTypeInfo TypeInfo { get; private set; }
        public PropertyValue Value { get; private set; }

        public IList<ValidationAttribute> ValidationAttributes
        {
            get
            {
                return PropertyInfo.GetCustomAttributes(false)
                  .OfType<ValidationAttribute>().ToList();
            }
        }

        public IDictionary<string, object> ControlsAttributes { get; set; }
        public string ForeignKeyName { get; private set; }

        public Property(Entity entity, PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Entity = entity;
            PropertyInfo = property;

            Name = property.Name;
            ColumnName = property.Name;
            ControlsAttributes = new Dictionary<string, object>();

            TypeInfo = new PropertyTypeInfo(property.PropertyType);
            Value = new PropertyValue(TypeInfo);

            if (TypeInfo.DataType == DataType.Numeric)
            {
                if (TypeInfo.IsFloatingPoint)
                {
                    ControlsAttributes.Add("data-number-number-of-decimals", "4");
                }
                if (TypeInfo.IsNullable)
                {
                    ControlsAttributes.Add("data-number-value", "");
                }
            }

            SetForeignKey();

            DisplayName = Name.SplitCamelCase();
            GroupName = IlaroAdminResources.Others;
        }

        internal void SetForeignKey(string foreignKeyName)
        {
            ForeignKeyName = foreignKeyName;
            if (ForeignKeyName.IsNullOrEmpty() == false)
            {
                IsForeignKey = true;

                if (TypeInfo.IsSystemType)
                {
                    ForeignEntityName = ForeignKeyName;
                }
                else
                {
                    ReferencePropertyName = ColumnName = ForeignKeyName;
                    ForeignEntityName = TypeInfo.Type.Name;
                }
            }
        }

        private void SetForeignKey()
        {
            if (TypeInfo.IsSystemType || TypeInfo.IsEnum)
            {
                IsForeignKey = false;
            }
            else
            {
                IsForeignKey = true;
                ForeignEntityName = TypeInfo.Type.Name;
            }
        }

        public MultiSelectList GetPossibleValues(bool addChooseItem = true)
        {
            if (IsForeignKey)
            {
                var options = new Dictionary<string, string>();

                if (addChooseItem)
                {
                    options.Add(String.Empty, IlaroAdminResources.Choose);
                }
                options = options.Union(Value.PossibleValues).ToDictionary(x => x.Key, x => x.Value);

                return TypeInfo.IsCollection ?
                    new MultiSelectList(options, "Key", "Value", Value.Values) :
                    new SelectList(options, "Key", "Value", Value.AsString);
            }
            else
            {
                var options = addChooseItem ?
                    TypeInfo.EnumType.GetOptions(String.Empty, IlaroAdminResources.Choose) :
                    TypeInfo.EnumType.GetOptions();

                if (TypeInfo.IsEnum)
                {
                    return new SelectList(
                        options,
                        "Key",
                        "Value",
                        Value.AsObject);
                }

                return new SelectList(options, "Key", "Value", Value.AsString);
            }
        }
    }
}
