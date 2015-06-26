using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.ViewModels;

namespace Ilaro.Admin.EntitiesFilters
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