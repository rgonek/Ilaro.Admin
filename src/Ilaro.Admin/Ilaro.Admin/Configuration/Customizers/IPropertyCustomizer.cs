using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.DataAnnotations;

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
        IPropertyCustomizer OnDelete(DeleteOption deleteOption);
        /// <summary>
        /// Set display and edit template
        /// </summary>
        IPropertyCustomizer Template(string display = null, string editor = null);
        /// <summary>
        /// Set data type
        /// </summary>
        IPropertyCustomizer Type(DataType dataType);
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
        IPropertyCustomizer DefaultValue(object value);
        /// <summary>
        /// Default value behavior
        /// </summary>
        IPropertyCustomizer DefaultValue(DefaultValueBehavior behavior);
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
    }
}