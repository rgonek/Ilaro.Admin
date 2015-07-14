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
        public Entity Entity { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public string Name { get; set; }

        private string _columnName;
        public string ColumnName
        {
            get { return _columnName; }
            set
            {
                if (value.StartsWith("[") && value.EndsWith("]"))
                    _columnName = value;
                else
                    _columnName = "[" + value + "]";
            }
        }

        public string DisplayName { get; set; }

        public string GroupName { get; set; }

        public string Description { get; set; }

        public string Prompt { get; set; }

        /// <summary>
        /// If property is a entity key.
        /// </summary>
        public bool IsKey { get; set; }

        public bool IsAutoKey
        {
            get { return TypeInfo.DataType != DataType.Text; }
        }

        /// <summary>
        /// If property is a link key.
        /// If you provide custom links to view a entity in your app, 
        /// that property is used to identify a entity.
        /// For example you have Product with slug, 
        /// so slug is used to display product not id.
        /// </summary>
        public bool IsLinkKey { get; set; }

        /// <summary>
        /// Is property is a foreign key.
        /// </summary>
        public bool IsForeignKey { get; set; }

        public Entity ForeignEntity { get; set; }

        public string ForeignEntityName { get; set; }

        public Property ReferenceProperty { get; set; }

        public string ReferencePropertyName { get; set; }

        public bool IsRequired { get; set; }
        public string RequiredErrorMessage { get; set; }

        public DeleteOption DeleteOption { get; internal set; }
        public FileOptions FileOptions { get; internal set; }
        public PropertyTemplate Template { get; internal set; }
        public PropertyTypeInfo TypeInfo { get; private set; }
        public PropertyValue Value { get; private set; }

        internal object[] Attributes
        {
            get { return PropertyInfo.GetCustomAttributes(false); }
        }

        public IList<ValidationAttribute> ValidationAttributes
        {
            get { return Attributes.OfType<ValidationAttribute>().ToList(); }
        }

        public IDictionary<string, object> ControlsAttributes { get; set; }

        public Property(Entity entity, PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            if (entity == null)
                throw new ArgumentNullException("entity");

            Entity = entity;
            PropertyInfo = property;

            Name = property.Name;
            ColumnName = property.Name;
            ControlsAttributes = new Dictionary<string, object>();
            // TODO: determine ColumnName

            TypeInfo = new PropertyTypeInfo(property.PropertyType, Attributes);
            FileOptions = new FileOptions(Attributes);
            Value = new PropertyValue(TypeInfo);
            Template = new PropertyTemplate(Attributes, TypeInfo, IsForeignKey);

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

            SetForeignKey(Attributes);

            SetDeleteOption(Attributes);

            IsKey = Attributes.OfType<KeyAttribute>().Any();
            IsLinkKey = Attributes.OfType<LinkKeyAttribute>().Any();

            var columnAttribute =
                Attributes.OfType<ColumnAttribute>().FirstOrDefault();
            if (columnAttribute != null)
            {
                ColumnName = columnAttribute.Name;
            }

            var requiredAttribute =
                Attributes.OfType<RequiredAttribute>().FirstOrDefault();
            if (requiredAttribute != null)
            {
                IsRequired = true;
                RequiredErrorMessage = requiredAttribute.ErrorMessage;
            }

            var displayAttribute =
                Attributes.OfType<DisplayAttribute>().FirstOrDefault();
            if (displayAttribute != null)
            {
                DisplayName = displayAttribute.Name ?? Name.SplitCamelCase();
                Description = displayAttribute.Description;
                GroupName =
                    displayAttribute.GroupName ?? IlaroAdminResources.Others;
            }
            else
            {
                DisplayName = Name.SplitCamelCase();
                GroupName = IlaroAdminResources.Others;
            }
        }

        private void SetDeleteOption(IEnumerable<object> attributes)
        {
            var onDeleteAttribute =
                attributes.OfType<OnDeleteAttribute>().FirstOrDefault();
            DeleteOption = onDeleteAttribute != null ?
                onDeleteAttribute.DeleteOption :
                DeleteOption.Nothing;
        }

        private void SetForeignKey(object[] attributes)
        {
            // move to other class, thanks that I can make a nice tests for this

            var foreignKeyAttribute =
                attributes.OfType<ForeignKeyAttribute>().FirstOrDefault();
            if (foreignKeyAttribute != null)
            {
                IsForeignKey = true;

                if (TypeInfo.IsSystemType)
                {
                    ForeignEntityName = foreignKeyAttribute.Name;
                }
                else
                {
                    ReferencePropertyName = ColumnName = foreignKeyAttribute.Name;
                    ForeignEntityName = TypeInfo.Type.Name;
                }
            }
            else
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
