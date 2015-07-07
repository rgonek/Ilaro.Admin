using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Core
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