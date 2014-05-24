using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.EntitiesFilters;

namespace Ilaro.Admin.ViewModels
{
    public class ListViewModel
    {
        public EntityViewModel Entity { get; set; }

        public IList<DataRowViewModel> Data { get; set; }

        public IList<ColumnViewModel> Columns { get; set; }

        public PagerInfo PagerInfo { get; set; }

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