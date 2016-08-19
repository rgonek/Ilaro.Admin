using System.Collections.Generic;

namespace Ilaro.Admin.Core.Filters
{
    public interface IFilterFactory
    {
        IEnumerable<BaseFilter> BuildFilters(EntityRecord entityRecord);
    }
}