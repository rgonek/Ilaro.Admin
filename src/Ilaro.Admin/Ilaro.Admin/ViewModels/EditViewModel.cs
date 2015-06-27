using Ilaro.Admin.Core;
using System.Collections.Generic;

namespace Ilaro.Admin.ViewModels
{
    public class EditViewModel
    {
        public Entity Entity { get; set; }

        public IList<GroupPropertiesViewModel> PropertiesGroups { get; set; }
    }
}