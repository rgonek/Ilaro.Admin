using Ilaro.Admin.Core.DataAccess;
using Ilaro.Admin.Core.File;
using System;

namespace Ilaro.Admin.Core.Configuration.Configurators
{
    public interface IPropertyConfigurator
    {
        /// <summary>
        /// Set SQL column name
        /// </summary>
        IPropertyConfigurator Column(string columnName);

        /// <summary>
        /// Set property display name and description
        /// </summary>
        IPropertyConfigurator Display(string name, string description = null);

        /// <summary>
        /// Mark property as primary key
        /// </summary>
        IPropertyConfigurator Id();

        /// <summary>
        /// Set delete behavior of foreign property on deleting
        /// </summary>
        IPropertyConfigurator Cascade(CascadeOption deleteOption);

        /// <summary>
        /// Set display and edit template
        /// </summary>
        IPropertyConfigurator Template(string display = null, string editor = null);

        /// <summary>
        /// Set data type
        /// </summary>
        IPropertyConfigurator Type(DataType dataType);

        /// <summary>
        /// Set enum type
        /// </summary>
        IPropertyConfigurator Enum(Type enumType);

        /// <summary>
        /// Make property searchable
        /// </summary>
        IPropertyConfigurator Searchable();

        /// <summary>
        /// Make property visible
        /// </summary>
        IPropertyConfigurator Visible(bool isVisible = true);

        /// <summary>
        /// Static default value for property
        /// </summary>
        IPropertyConfigurator OnCreate(object value);

        /// <summary>
        /// Default value behavior
        /// </summary>
        IPropertyConfigurator OnCreate(ValueBehavior behavior);

        /// <summary>
        /// Static default value for property
        /// </summary>
        IPropertyConfigurator OnUpdate(object value);

        /// <summary>
        /// Default value behavior
        /// </summary>
        IPropertyConfigurator OnUpdate(ValueBehavior behavior);

        /// <summary>
        /// Set value for OnCreate and OnUpdate
        /// </summary>
        IPropertyConfigurator OnSave(object value);

        /// <summary>
        /// Set value for OnCreate and OnUpdate
        /// </summary>
        IPropertyConfigurator OnSave(ValueBehavior behavior);

        /// <summary>
        /// Static default value for property. Used only when soft delete for entity is enabled.
        /// </summary>
        IPropertyConfigurator OnDelete(object value);

        /// <summary>
        /// Default value behavior. Used only when soft delete for entity is enabled.
        /// </summary>
        IPropertyConfigurator OnDelete(ValueBehavior behavior);

        /// <summary>
        /// Set image options
        /// </summary>
        IPropertyConfigurator Image(string path, int? width, int? height);

        /// <summary>
        /// Set image options
        /// </summary>
        IPropertyConfigurator File(
            NameCreation nameCreation,
            long maxFileSize,
            bool isImage,
            string path,
            params string[] allowedFileExtensions);

        /// <summary>
        /// Set data format string
        /// </summary>
        IPropertyConfigurator Format(string dataFormatString);

        /// <summary>
        /// Set property as a foreign key
        /// </summary>
        IPropertyConfigurator ForeignKey(string name);

        /// <summary>
        /// Mark property as timestamp
        /// </summary>
        IPropertyConfigurator IsTimestamp();

        /// <summary>
        /// Mark property as concurrency check
        /// </summary>
        IPropertyConfigurator IsConcurrencyCheck();

        /// <summary>
        /// Set default order for property
        /// </summary>
        IPropertyConfigurator DefaultOrder(OrderDirection orderType = OrderDirection.Asc);

        /// <summary>
        /// Set default filter for property
        /// </summary>
        IPropertyConfigurator DefaultFilter(ValueBehavior behavior);

        /// <summary>
        /// Set default filter for property
        /// </summary>
        IPropertyConfigurator DefaultFilter(object value);

        /// <summary>
        /// Make property filterable
        /// </summary>
        IPropertyConfigurator Filterable();

        IPropertyConfigurator ManyToMany();

        IPropertyConfigurator MultiValue(string separator);
    }
}