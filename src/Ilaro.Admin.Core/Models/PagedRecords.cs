using Ilaro.Admin.Core.Filters;
using System.Collections.Generic;

namespace Ilaro.Admin.Core.Models
{
    public class PagedRecords
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public IList<EntityRecord> Records { get; set; }
        public IList<BaseFilter> Filters { get; set; }
    }
}
