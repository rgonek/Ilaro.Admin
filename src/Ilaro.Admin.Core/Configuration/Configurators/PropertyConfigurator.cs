using Ilaro.Admin.Core.DataAccess;
using Ilaro.Admin.Core.File;
using System;
using System.Collections.Generic;

namespace Ilaro.Admin.Core.Configuration.Configurators
{
    public class PropertyConfigurator : IPropertyConfigurator
    {
        private PropertyConfigurationHolder propertyCustomizerHolder;

        public PropertyConfigurator(PropertyConfigurationHolder propertyCustomizerHolder)
        {
            if (propertyCustomizerHolder == null)
                throw new ArgumentNullException(nameof(propertyCustomizerHolder));

            this.propertyCustomizerHolder = propertyCustomizerHolder;
        }

        public IPropertyConfigurator Column(string columnName)
        {
            propertyCustomizerHolder.Column = columnName;

            return this;
        }

        public IPropertyConfigurator OnCreate(ValueBehavior behavior)
        {
            propertyCustomizerHolder.OnCreateDefaultValue = behavior;
            propertyCustomizerHolder.IsCreatable = false;

            return this;
        }

        public IPropertyConfigurator OnCreate(object value)
        {
            propertyCustomizerHolder.OnCreateDefaultValue = value;
            propertyCustomizerHolder.IsCreatable = false;

            return this;
        }

        public IPropertyConfigurator OnUpdate(ValueBehavior behavior)
        {
            propertyCustomizerHolder.OnUpdateDefaultValue = behavior;
            propertyCustomizerHolder.IsCreatable = false;

            return this;
        }

        public IPropertyConfigurator OnUpdate(object value)
        {
            propertyCustomizerHolder.OnUpdateDefaultValue = value;
            propertyCustomizerHolder.IsCreatable = false;

            return this;
        }

        public IPropertyConfigurator OnSave(ValueBehavior behavior)
        {
            OnCreate(behavior);
            OnUpdate(behavior);

            return this;
        }

        public IPropertyConfigurator OnSave(object value)
        {
            OnCreate(value);
            OnUpdate(value);

            return this;
        }

        public IPropertyConfigurator OnDelete(ValueBehavior behavior)
        {
            propertyCustomizerHolder.OnDeleteDefaultValue = behavior;

            return this;
        }

        public IPropertyConfigurator OnDelete(object value)
        {
            propertyCustomizerHolder.OnDeleteDefaultValue = value;

            return this;
        }

        public IPropertyConfigurator Display(string name, string description = null)
        {
            propertyCustomizerHolder.DisplayName = name;
            propertyCustomizerHolder.Description = description;

            return this;
        }

        public IPropertyConfigurator File(
            NameCreation nameCreation,
            long maxFileSize,
            bool isImage,
            string path,
            params string[] allowedFileExtensions)
        {
            if (propertyCustomizerHolder.FileOptions == null)
            {
                propertyCustomizerHolder.FileOptions = new FileOptions
                {
                    Settings = new List<ImageSettings>()
                };
            }

            propertyCustomizerHolder.FileOptions.NameCreation = nameCreation;
            propertyCustomizerHolder.FileOptions.MaxFileSize = maxFileSize;
            propertyCustomizerHolder.FileOptions.IsImage = isImage;
            propertyCustomizerHolder.FileOptions.Path = path;
            propertyCustomizerHolder.FileOptions.AllowedFileExtensions = allowedFileExtensions;

            return Type(isImage ? Core.DataType.Image : Core.DataType.File);
        }

        public IPropertyConfigurator ForeignKey(string name)
        {
            propertyCustomizerHolder.IsForeignKey = true;
            propertyCustomizerHolder.ForeignKey = name;

            return this;
        }

        public IPropertyConfigurator Format(string dataFormatString)
        {
            propertyCustomizerHolder.Format = dataFormatString;

            return this;
        }

        public IPropertyConfigurator Id()
        {
            propertyCustomizerHolder.IsKey = true;
            propertyCustomizerHolder.IsRequired = true;


            return this;
        }

        public IPropertyConfigurator Image(string path, int? width, int? height)
        {
            if (propertyCustomizerHolder.FileOptions == null)
            {
                propertyCustomizerHolder.FileOptions = new FileOptions
                {
                    NameCreation = NameCreation.OriginalFileName,
                    Settings = new List<ImageSettings>()
                };
            }

            propertyCustomizerHolder.FileOptions.Settings.Add(new ImageSettings(path, width, height));

            return Type(Core.DataType.Image);
        }

        public IPropertyConfigurator Cascade(CascadeOption deleteOption)
        {
            propertyCustomizerHolder.CascadeOption = deleteOption;

            return this;
        }

        public IPropertyConfigurator Searchable()
        {
            propertyCustomizerHolder.IsSearchable = true;

            return this;
        }

        public IPropertyConfigurator Filterable()
        {
            propertyCustomizerHolder.IsFilterable = true;

            return this;
        }

        public IPropertyConfigurator Template(string display = null, string editor = null)
        {
            propertyCustomizerHolder.DisplayTemplate = display;
            propertyCustomizerHolder.EditorTemplate = editor;

            return this;
        }

        public IPropertyConfigurator Type(Core.DataType dataType)
        {
            propertyCustomizerHolder.DataType = dataType;

            return this;
        }

        public IPropertyConfigurator Enum(Type enumType)
        {
            propertyCustomizerHolder.EnumType = enumType;

            return Type(Core.DataType.Enum);
        }

        public IPropertyConfigurator Visible(bool isVisible = true)
        {
            propertyCustomizerHolder.IsVisible = isVisible;

            return this;
        }
        public IPropertyConfigurator IsTimestamp()
        {
            propertyCustomizerHolder.IsTimestamp = true;
            propertyCustomizerHolder.IsCreatable = false;
            Visible(false);
            IsConcurrencyCheck();
            Type(Core.DataType.Binary);

            return this;
        }

        public IPropertyConfigurator IsConcurrencyCheck()
        {
            propertyCustomizerHolder.IsConcurrencyCheck = true;

            return this;
        }

        public IPropertyConfigurator DefaultOrder(OrderDirection orderType = OrderDirection.Asc)
        {
            propertyCustomizerHolder.DefaultOrder = orderType;

            return this;
        }

        public IPropertyConfigurator DefaultFilter(ValueBehavior behavior)
        {
            propertyCustomizerHolder.DefaultFilter = behavior;

            return this;
        }

        public IPropertyConfigurator DefaultFilter(object value)
        {
            propertyCustomizerHolder.DefaultFilter = value;

            return this;
        }

        public IPropertyConfigurator ManyToMany()
        {
            propertyCustomizerHolder.IsManyToMany = true;

            return this;
        }

        public IPropertyConfigurator MultiValue(string separator)
        {
            propertyCustomizerHolder.MultiValueSeparator = separator;

            return this;
        }
    }
}