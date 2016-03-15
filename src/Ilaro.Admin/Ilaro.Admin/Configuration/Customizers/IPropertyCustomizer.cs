using Ilaro.Admin.Core;

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
        IPropertyCustomizer Display(string singular, string plural);
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
        IPropertyCustomizer Searchable();
        IPropertyCustomizer Visible();
        /// <summary>
        /// Set image options
        /// </summary>
        IPropertyCustomizer Image();
        /// <summary>
        /// Set image options
        /// </summary>
        IPropertyCustomizer File();
    }
}