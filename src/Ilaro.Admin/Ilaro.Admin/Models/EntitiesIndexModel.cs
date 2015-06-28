using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Filters;

namespace Ilaro.Admin.Models
{
    public class EntitiesIndexModel
    {
        public Entity Entity { get; set; }

        public IList<DataRow> Data { get; set; }

        public IList<Column> Columns { get; set; }

        public PagerInfo Pager { get; set; }

        public IList<IEntityFilter> Filters { get; set; }

        public IList<IEntityFilter> ActiveFilters
        {
            get
            {
                return Filters.Where(x => !x.Value.IsNullOrEmpty()).ToList();
            }
        }

        public string SearchQuery { get; set; }

        public bool IsSearchActive { get; set; }

        public int PerPage { get; set; }

        public string Order { get; set; }

        public string OrderDirection { get; set; }
    }
}
