using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.DataAnnotations;
using Ilaro.Admin.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SystemDataType = System.ComponentModel.DataAnnotations.DataType;

namespace Ilaro.Admin.Configuration.Customizers
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

            return this;
        }

        public IPropertyCustomizer OnCreate(object value)
        {
            propertyCustomizerHolder.OnCreateDefaultValue = value;

            return this;
        }

        public IPropertyCustomizer OnUpdate(ValueBehavior behavior)
        {
            propertyCustomizerHolder.OnUpdateDefaultValue = behavior;

            return this;
        }

        public IPropertyCustomizer OnUpdate(object value)
        {
            propertyCustomizerHolder.OnUpdateDefaultValue = value;

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

        public IPropertyCustomizer Display(string name, string description)
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

        public IPropertyCustomizer Visible()
        {
            propertyCustomizerHolder.IsVisible = true;

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
    }
}