using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.ViewModels
{
    public class CreateViewModel
    {
        public Entity Entity { get; set; }

        public IList<GroupPropertiesViewModel> PropertiesGroups { get; set; }
    }
}