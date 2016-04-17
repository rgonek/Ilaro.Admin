using System.Collections.Generic;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Filters
{
    public interface IFilterFactory
    {
        IEnumerable<BaseFilter> BuildFilters(EntityRecord entityRecord);
    }
}