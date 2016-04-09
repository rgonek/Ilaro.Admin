using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SystemDataType = System.ComponentModel.DataAnnotations.DataType;

namespace Ilaro.Admin.Configuration.Customizers
{
    public interface IPropertyCustomizer
    {
        /// <summary>
        /// Set SQL column name
        /// </summary>
        IPropertyCustomizer Column(string columnName);
        /// <summary>
        /// Set property display name and description
        /// </summary>
        IPropertyCustomizer Display(string name, string description);
        /// <summary>
        /// Mark property as primary key
        /// </summary>
        IPropertyCustomizer Id();
        /// <summary>
        /// Set delete behavior of foreign property on deleting
        /// </summary>
        IPropertyCustomizer Cascade(CascadeOption deleteOption);
        /// <summary>
        /// Set display and edit template
        /// </summary>
        IPropertyCustomizer Template(string display = null, string editor = null);
        /// <summary>
        /// Set data type
        /// </summary>
        IPropertyCustomizer Type(Core.DataType dataType);
        /// <summary>
        /// Set data annotation data type
        /// </summary>
        IPropertyCustomizer Type(SystemDataType dataType, string errorMessage = null);
        /// <summary>
        /// Set enum type
        /// </summary>
        IPropertyCustomizer Enum(Type enumType);
        /// <summary>
        /// Make property searchable
        /// </summary>
        IPropertyCustomizer Searchable();
        /// <summary>
        /// Make property visible
        /// </summary>
        IPropertyCustomizer Visible();
        /// <summary>
        /// Static default value for property
        /// </summary>
        IPropertyCustomizer OnCreate(object value);
        /// <summary>
        /// Default value behavior
        /// </summary>
        IPropertyCustomizer OnCreate(ValueBehavior behavior);
        /// <summary>
        /// Static default value for property
        /// </summary>
        IPropertyCustomizer OnUpdate(object value);
        /// <summary>
        /// Default value behavior
        /// </summary>
        IPropertyCustomizer OnUpdate(ValueBehavior behavior);
        /// <summary>
        /// Static default value for property. Used only when soft delete for entity is enabled.
        /// </summary>
        IPropertyCustomizer OnDelete(object value);
        /// <summary>
        /// Default value behavior. Used only when soft delete for entity is enabled.
        /// </summary>
        IPropertyCustomizer OnDelete(ValueBehavior behavior);
        /// <summary>
        /// Set image options
        /// </summary>
        IPropertyCustomizer Image(string path, int? width, int? height);
        /// <summary>
        /// Set image options
        /// </summary>
        IPropertyCustomizer File(
            NameCreation nameCreation,
            long maxFileSize,
            bool isImage,
            string path,
            params string[] allowedFileExtensions);
        /// <summary>
        /// Set data format string
        /// </summary>
        IPropertyCustomizer Format(string dataFormatString);
        /// <summary>
        /// Set property as a foreign key
        /// </summary>
        IPropertyCustomizer ForeignKey(string name);
        /// <summary>
        /// Set validators
        /// </summary>
        IPropertyCustomizer Validators(IEnumerable<ValidationAttribute> validators);
        /// <summary>
        /// Set validator
        /// </summary>
        IPropertyCustomizer Validator(ValidationAttribute validator);
        /// <summary>
        /// Property is required
        /// </summary>
        IPropertyCustomizer Required(string errorMessage = null);
        /// <summary>
        /// String length property validator
        /// </summary>
        IPropertyCustomizer StringLength(
            int maximumLength,
            int minimumLength = 0,
            string errorMessage = null);
        /// <summary>
        /// Compare property validator
        /// </summary>
        IPropertyCustomizer Compare(
            string otherProperty,
            string errorMessage = null);
        /// <summary>
        /// Range property validator
        /// </summary>
        IPropertyCustomizer Range(int minimum, int maximum, string errorMessage = null);
        /// <summary>
        /// Range property validator
        /// </summary>
        IPropertyCustomizer Range(double minimum, double maximum, string errorMessage = null);
        /// <summary>
        /// Range property validator
        /// </summary>
        IPropertyCustomizer Range(Type type, string minimum, string maximum, string errorMessage = null);
        /// <summary>
        /// Regular expression property validator
        /// </summary>
        IPropertyCustomizer RegularExpression(string pattern, string errorMessage = null);
    }
}