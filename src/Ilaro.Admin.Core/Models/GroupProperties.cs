using System.Collections.Generic;

namespace Ilaro.Admin.Core.Models
{
    public class GroupProperties
    {
        public string GroupName { get; set; }
        public IEnumerable<PropertyValue> PropertiesValues { get; set; }
        public bool IsCollapsed { get; set; }
    }
}
