using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

        private string _column;
        public string Column
        {
            get { return _column; }
            internal set
            {
                if (value.StartsWith("[") && value.EndsWith("]"))
                    _column = value;
                else
                    _column = "[" + value + "]";
            }
        }

        public string Display { get; internal set; }

        public string Group { get; internal set; }

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

        public CascadeOption CascadeOption { get; internal set; }
        public FileOptions FileOptions { get; internal set; } = new FileOptions();
        public PropertyTemplate Template { get; internal set; } = new PropertyTemplate();
        public PropertyTypeInfo TypeInfo { get; private set; }

        public IList<ValidationAttribute> Validators { get; internal set; } = new List<ValidationAttribute>();

        public IDictionary<string, object> ControlsAttributes { get; set; }
        public string ForeignKeyName { get; private set; }
        public object OnCreateDefaultValue { get; internal set; }
        public object OnUpdateDefaultValue { get; internal set; }
        public object OnDeleteDefaultValue { get; internal set; }

        public Property(Entity entity, PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Entity = entity;
            PropertyInfo = property;

            Name = property.Name;
            Column = property.Name;
            ControlsAttributes = new Dictionary<string, object>();

            TypeInfo = new PropertyTypeInfo(property.PropertyType);

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

            Display = Name.SplitCamelCase();
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
                    ReferencePropertyName = Column = ForeignKeyName;
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
    }
}
