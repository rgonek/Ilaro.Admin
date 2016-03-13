using Ilaro.Admin.Core;

namespace Ilaro.Admin.Configuration.Customizers
{
    public class PropertyCustomizerHolder
    {
        public string Column { get; internal set; }
        public string EditorTemplate { get; internal set; }
        public string NameSingular { get; internal set; }
        public string DisplayTemplate { get; internal set; }
        public bool IsKey { get; internal set; }
        public DeleteOption OnDelete { get; internal set; }
        public bool IsSearchable { get; internal set; }
        public DataType DataType { get; internal set; }
        public bool IsVisible { get; internal set; }
        public string NamePlural { get; internal set; }
    }
}