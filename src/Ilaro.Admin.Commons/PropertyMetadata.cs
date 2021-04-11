namespace Ilaro.Admin.Common
{
    public class PropertyMetadata
    {
        public string Name { get; internal set; }

        public bool IsConcurrencyCheck { get; internal set; }

        public bool IsKey { get; internal set; }

        public bool IsVisible { get; internal set; }

        public string Format { get; set; }

        public PropertyTypeInfo TypeInfo { get; }

        public string Column { get; set; }

        public bool IsAutoKey { get; set; }
        public bool IsManyToMany { get; set; }
    }
}
