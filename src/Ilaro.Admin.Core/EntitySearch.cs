using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Core.Extensions;

namespace Ilaro.Admin.Core
{
    public class EntitySearch
    {
        public string Query { get; }

        public IEnumerable<Property> Properties { get; }

        public bool IsActive => Query.HasValue() && Properties.Any();

        public EntitySearch(string query, IEnumerable<Property> properties)
            => (Query, Properties) = (query, properties);
    }
}