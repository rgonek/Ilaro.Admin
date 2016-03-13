using System;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Configuration.Customizers
{
    public class PropertyCustomizer : IPropertyCustomizer
    {
        private PropertyCustomizerHolder propertyCustomizerHolder;

        public PropertyCustomizer(PropertyCustomizerHolder propertyCustomizerHolder)
        {
            if (propertyCustomizerHolder == null)
                throw new ArgumentNullException("propertyCustomizerHolder");

            this.propertyCustomizerHolder = propertyCustomizerHolder;
        }

        public void Column(string columnName)
        {
            propertyCustomizerHolder.Column = columnName;
        }

        public void Display(string singular, string plural)
        {
            propertyCustomizerHolder.NameSingular = singular;
            propertyCustomizerHolder.NamePlural = plural;
        }

        public void File()
        {

        }

        public void Id()
        {
            propertyCustomizerHolder.IsKey = true;
        }

        public void Image()
        {
        }

        public void OnDelete(DeleteOption deleteOption)
        {
            propertyCustomizerHolder.OnDelete = deleteOption;
        }

        public void Searchable()
        {
            propertyCustomizerHolder.IsSearchable = true;
        }

        public void Template(string display = null, string editor = null)
        {
            propertyCustomizerHolder.DisplayTemplate = display;
            propertyCustomizerHolder.EditorTemplate = editor;
        }

        public void Type(DataType dataType)
        {
            propertyCustomizerHolder.DataType = dataType;
        }

        public void Visible()
        {
            propertyCustomizerHolder.IsVisible = true;
        }
    }
}