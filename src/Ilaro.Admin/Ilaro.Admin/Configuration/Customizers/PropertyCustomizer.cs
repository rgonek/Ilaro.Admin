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

        public IPropertyCustomizer Column(string columnName)
        {
            propertyCustomizerHolder.Column = columnName;

            return this;
        }

        public IPropertyCustomizer Display(string singular, string plural)
        {
            propertyCustomizerHolder.NameSingular = singular;
            propertyCustomizerHolder.NamePlural = plural;

            return this;
        }

        public IPropertyCustomizer File()
        {

            return this;
        }

        public IPropertyCustomizer Id()
        {
            propertyCustomizerHolder.IsKey = true;

            return this;
        }

        public IPropertyCustomizer Image()
        {

            return this;
        }

        public IPropertyCustomizer OnDelete(DeleteOption deleteOption)
        {
            propertyCustomizerHolder.OnDelete = deleteOption;

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

        public IPropertyCustomizer Type(DataType dataType)
        {
            propertyCustomizerHolder.DataType = dataType;

            return this;
        }

        public IPropertyCustomizer Visible()
        {
            propertyCustomizerHolder.IsVisible = true;

            return this;
        }
    }
}