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

        public IList<BaseFilter> Filters { get; set; }

        public IList<BaseFilter> ActiveFilters
        {
            get
            {
                return Filters.Where(x => x.IsActive).ToList();
            }
        }

        public TableInfo TableInfo { get; set; }

        public IConfiguration Configuration { get; set; }
    }
}
