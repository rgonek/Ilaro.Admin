using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Filters
{
    public class EntitySearch
    {
        public string Query { get; set; }

        public IEnumerable<Property> Properties { get; set; }

        public bool IsActive
        {
            get
            {
                return !Query.IsNullOrEmpty() && Properties.Any();
            }
        }
    }
}