using Ilaro.Admin.Core.Filters;

namespace Ilaro.Admin.Core.Models
{
    public class WrappedFilterOption
    {
        public BaseFilter Filter { get; set; }
        //public SelectListItem Option { get; set; }

        public WrappedFilterOption(BaseFilter filter)//, SelectListItem option)
        {
            Filter = filter;
            //Option = option;
        }
    }
}