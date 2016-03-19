using System;

namespace Ilaro.Admin.DataAnnotations
{
    /// <summary>
    /// Attribute describes a display format for record.
    /// For example: "Product name: {ProductName}"
    /// In this example {ProductName} is name of property in entity, 
    /// and it be replaced by it's value
    /// It's used for displaying editors for foreign entity
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RecordDisplayAttribute : Attribute
    {
        public string DisplayFormat { get; set; }

        public RecordDisplayAttribute(string displayFormat)
        {
            DisplayFormat = displayFormat;
        }
    }
}