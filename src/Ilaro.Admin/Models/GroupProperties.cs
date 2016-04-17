using System.Collections.Generic;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Models
{
    public class GroupProperties
    {
        public string GroupName { get; set; }
        public IEnumerable<PropertyValue> PropertiesValues { get; set; }
        public bool IsCollapsed { get; set; }
    }
}
