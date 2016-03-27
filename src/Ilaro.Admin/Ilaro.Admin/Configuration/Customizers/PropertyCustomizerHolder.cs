using System;
using Ilaro.Admin.Core;
using Ilaro.Admin.DataAnnotations;
using SystemDataType = System.ComponentModel.DataAnnotations.DataType;

namespace Ilaro.Admin.Configuration.Customizers
{
    public class PropertyCustomizerHolder
    {
        public string Column { get; internal set; }
        public string EditorTemplate { get; internal set; }
        public string DisplayName { get; internal set; }
        public string DisplayTemplate { get; internal set; }
        public bool? IsKey { get; internal set; }
        public DeleteOption? DeleteOption { get; internal set; }
        public bool? IsSearchable { get; internal set; }
        public DataType? DataType { get; internal set; }
        public bool? IsVisible { get; internal set; }
        public string Description { get; internal set; }
        public FileOptions FileOptions { get; internal set; }
        public string Group { get; internal set; }
        public object DefaultValue { get; internal set; }
        public string Format { get; internal set; }
        public string RequiredErrorMessage { get; internal set; }
        public bool IsRequired { get; internal set; }
        public Type EnumType { get; internal set; }
        public SystemDataType? SourceDataType { get; internal set; }
        public string ForeignKey { get; internal set; }
        public bool IsForeignKey { get; internal set; }
    }
}