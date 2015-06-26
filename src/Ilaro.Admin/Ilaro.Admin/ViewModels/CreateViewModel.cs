using System.Collections.Generic;

namespace Ilaro.Admin.ViewModels
{
    public class CreateViewModel
    {
        public Entity Entity { get; set; }

        public IList<GroupPropertiesViewModel> PropertiesGroups { get; set; }
    }
}