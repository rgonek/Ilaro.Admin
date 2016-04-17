using Ilaro.Admin.Filters;
using System.Collections.Generic;

namespace Ilaro.Admin.Models
{
    public class PagedRecords
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public IList<DataRow> Records { get; set; }
        public IList<BaseFilter> Filters { get; set; }
    }
}
