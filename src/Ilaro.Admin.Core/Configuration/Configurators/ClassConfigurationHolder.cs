﻿using System.Collections.Generic;

namespace Ilaro.Admin.Core.Configuration.Configurators
{
    public class ClassConfigurationHolder
    {
        public string DeleteLink { get; internal set; }

        public string DisplayFormat { get; internal set; }

        public string DisplayLink { get; internal set; }

        public string EditLink { get; internal set; }

        public string Group { get; internal set; }

        public string NamePlural { get; internal set; }

        public string NameSingular { get; internal set; }

        public string Table { get; internal set; }

        public string Schema { get; internal set; }

        public IDictionary<string, bool> PropertiesGroups { get; }
            = new Dictionary<string, bool>();

        public bool? AllowDelete { get; internal set; }

        public bool? AllowEdit { get; internal set; }

        public bool SoftDeleteEnabled { get; internal set; }

        public bool? ConcurrencyCheckEnabled { get; internal set; }
    }
}