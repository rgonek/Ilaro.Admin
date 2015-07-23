using System.Web.Mvc;
using Ilaro.Admin.Filters;

namespace Ilaro.Admin.Models
{
    public class WrappedFilterOption
    {
        public BaseFilter Filter { get; set; }
        public SelectListItem Option { get; set; }

        public WrappedFilterOption(BaseFilter filter, SelectListItem option)
        {
            Filter = filter;
            Option = option;
        }
    }
}