﻿using System;
using Ilaro.Admin.Core.File;
using SystemDataType = System.ComponentModel.DataAnnotations.DataType;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ilaro.Admin.Core.Configuration.Configurators
{
    public class PropertyConfigurationHolder
    {
        public string Column { get; internal set; }

        public string EditorTemplate { get; internal set; }

        public string DisplayName { get; internal set; }

        public string DisplayTemplate { get; internal set; }

        public bool? IsKey { get; internal set; }

        public CascadeOption? CascadeOption { get; internal set; }

        public bool? IsSearchable { get; internal set; }

        public DataType? DataType { get; internal set; }

        public bool? IsVisible { get; internal set; }

        public string Description { get; internal set; }

        public FileOptions FileOptions { get; internal set; }

        public string Group { get; internal set; }

        public object OnCreateDefaultValue { get; internal set; }

        public object OnUpdateDefaultValue { get; internal set; }

        public string Format { get; internal set; }

        public string RequiredErrorMessage { get; internal set; }

        public bool? IsRequired { get; internal set; }

        public Type EnumType { get; internal set; }

        public string ForeignKey { get; internal set; }

        public bool IsForeignKey { get; internal set; }

        public object OnDeleteDefaultValue { get; internal set; }

        public bool IsTimestamp { get; internal set; }

        public bool IsCreatable { get; internal set; } = true;

        public bool IsConcurrencyCheck { get; internal set; }

        public OrderDirection? DefaultOrder { get; internal set; }

        public object DefaultFilter { get; internal set; }

        public bool? IsFilterable { get; internal set; }

        public bool IsManyToMany { get; internal set; }

        public string MultiValueSeparator { get; internal set; }
    }
}