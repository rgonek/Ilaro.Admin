﻿using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Core.DataAnnotations;
using Ilaro.Admin.Core.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SystemDataType = System.ComponentModel.DataAnnotations.DataType;

namespace Ilaro.Admin.Core.Customization.Customizers
{
    public class PropertyCustomizer : IPropertyCustomizer
    {
        private PropertyCustomizerHolder propertyCustomizerHolder;

        public PropertyCustomizer(PropertyCustomizerHolder propertyCustomizerHolder)
        {
            if (propertyCustomizerHolder == null)
                throw new ArgumentNullException(nameof(propertyCustomizerHolder));

            this.propertyCustomizerHolder = propertyCustomizerHolder;
        }

        public IPropertyCustomizer Column(string columnName)
        {
            propertyCustomizerHolder.Column = columnName;

            return this;
        }

        public IPropertyCustomizer OnCreate(ValueBehavior behavior)
        {
            propertyCustomizerHolder.OnCreateDefaultValue = behavior;
            propertyCustomizerHolder.IsCreatable = false;

            return this;
        }

        public IPropertyCustomizer OnCreate(object value)
        {
            propertyCustomizerHolder.OnCreateDefaultValue = value;
            propertyCustomizerHolder.IsCreatable = false;

            return this;
        }

        public IPropertyCustomizer OnUpdate(ValueBehavior behavior)
        {
            propertyCustomizerHolder.OnUpdateDefaultValue = behavior;
            propertyCustomizerHolder.IsCreatable = false;

            return this;
        }

        public IPropertyCustomizer OnUpdate(object value)
        {
            propertyCustomizerHolder.OnUpdateDefaultValue = value;
            propertyCustomizerHolder.IsCreatable = false;

            return this;
        }

        public IPropertyCustomizer OnSave(ValueBehavior behavior)
        {
            OnCreate(behavior);
            OnUpdate(behavior);

            return this;
        }

        public IPropertyCustomizer OnSave(object value)
        {
            OnCreate(value);
            OnUpdate(value);

            return this;
        }

        public IPropertyCustomizer OnDelete(ValueBehavior behavior)
        {
            propertyCustomizerHolder.OnDeleteDefaultValue = behavior;

            return this;
        }

        public IPropertyCustomizer OnDelete(object value)
        {
            propertyCustomizerHolder.OnDeleteDefaultValue = value;

            return this;
        }

        public IPropertyCustomizer Display(string name, string description = null)
        {
            propertyCustomizerHolder.DisplayName = name;
            propertyCustomizerHolder.Description = description;

            return this;
        }

        public IPropertyCustomizer File(
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

        public IPropertyCustomizer ForeignKey(string name)
        {
            propertyCustomizerHolder.IsForeignKey = true;
            propertyCustomizerHolder.ForeignKey = name;

            return this;
        }

        public IPropertyCustomizer Format(string dataFormatString)
        {
            propertyCustomizerHolder.Format = dataFormatString;

            return this;
        }

        public IPropertyCustomizer Id()
        {
            propertyCustomizerHolder.IsKey = true;
            propertyCustomizerHolder.IsRequired = true;


            return this;
        }

        public IPropertyCustomizer Image(string path, int? width, int? height)
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

        public IPropertyCustomizer Cascade(CascadeOption deleteOption)
        {
            propertyCustomizerHolder.CascadeOption = deleteOption;

            return this;
        }

        public IPropertyCustomizer Required(string errorMessage = null)
        {
            propertyCustomizerHolder.IsRequired = true;
            Validator(new RequiredAttribute()
                .SetErrorMessage(errorMessage));

            return this;
        }

        public IPropertyCustomizer Searchable()
        {
            propertyCustomizerHolder.IsSearchable = true;

            return this;
        }

        public IPropertyCustomizer Filterable()
        {
            propertyCustomizerHolder.IsFilterable = true;

            return this;
        }

        public IPropertyCustomizer Template(string display = null, string editor = null)
        {
            propertyCustomizerHolder.DisplayTemplate = display;
            propertyCustomizerHolder.EditorTemplate = editor;

            return this;
        }

        public IPropertyCustomizer Type(Core.DataType dataType)
        {
            propertyCustomizerHolder.DataType = dataType;

            return this;
        }

        public IPropertyCustomizer Type(SystemDataType dataType, string errorMessage = null)
        {
            propertyCustomizerHolder.SourceDataType = dataType;
            Validator(new DataTypeAttribute(dataType)
                .SetErrorMessage(errorMessage));

            return Type(DataTypeConverter.Convert(dataType));
        }

        public IPropertyCustomizer Enum(Type enumType)
        {
            propertyCustomizerHolder.EnumType = enumType;

            return Type(Core.DataType.Enum);
        }

        public IPropertyCustomizer Visible(bool isVisible = true)
        {
            propertyCustomizerHolder.IsVisible = isVisible;

            return this;
        }

        public IPropertyCustomizer Validators(
            IEnumerable<ValidationAttribute> validators)
        {
            if (propertyCustomizerHolder.Validators == null)
            {
                propertyCustomizerHolder.Validators = validators;
            }
            else
            {
                propertyCustomizerHolder.Validators =
                    propertyCustomizerHolder.Validators.Union(validators);
            }

            return this;
        }

        public IPropertyCustomizer Validator(ValidationAttribute validator)
        {
            Validators(new[] { validator });

            return this;
        }

        public IPropertyCustomizer StringLength(
            int maximumLength,
            int minimumLength = 0,
            string errorMessage = null)
        {
            Validator(new StringLengthAttribute(maximumLength)
            {
                MinimumLength = minimumLength
            }.SetErrorMessage(errorMessage));

            return this;
        }

        public IPropertyCustomizer Compare(
            string otherProperty,
            string errorMessage = null)
        {
            Validator(new CompareAttribute(otherProperty)
                .SetErrorMessage(errorMessage));

            return this;
        }

        public IPropertyCustomizer Range(int minimum, int maximum, string errorMessage = null)
        {
            Validator(new RangeAttribute(minimum, maximum)
                .SetErrorMessage(errorMessage));

            return this;
        }

        public IPropertyCustomizer Range(double minimum, double maximum, string errorMessage = null)
        {
            Validator(new RangeAttribute(minimum, maximum)
                .SetErrorMessage(errorMessage));

            return this;
        }

        public IPropertyCustomizer Range(Type type, string minimum, string maximum, string errorMessage = null)
        {
            Validator(new RangeAttribute(type, minimum, maximum)
                .SetErrorMessage(errorMessage));

            return this;
        }

        public IPropertyCustomizer RegularExpression(string pattern, string errorMessage = null)
        {
            Validator(new RegularExpressionAttribute(pattern)
                .SetErrorMessage(errorMessage));

            return this;
        }

        public IPropertyCustomizer IsTimestamp()
        {
            propertyCustomizerHolder.IsTimestamp = true;
            propertyCustomizerHolder.IsCreatable = false;
            Visible(false);
            IsConcurrencyCheck();
            Type(Core.DataType.Binary);

            return this;
        }

        public IPropertyCustomizer IsConcurrencyCheck()
        {
            propertyCustomizerHolder.IsConcurrencyCheck = true;

            return this;
        }

        public IPropertyCustomizer DefaultOrder(OrderType orderType = OrderType.Asc)
        {
            propertyCustomizerHolder.DefaultOrder = orderType;

            return this;
        }

        public IPropertyCustomizer DefaultFilter(ValueBehavior behavior)
        {
            propertyCustomizerHolder.DefaultFilter = behavior;

            return this;
        }

        public IPropertyCustomizer DefaultFilter(object value)
        {
            propertyCustomizerHolder.DefaultFilter = value;

            return this;
        }

        public IPropertyCustomizer ManyToMany()
        {
            propertyCustomizerHolder.IsManyToMany = true;

            return this;
        }

        public IPropertyCustomizer MultiValue(string separator)
        {
            propertyCustomizerHolder.MultiValueSeparator = separator;

            return this;
        }
    }
}