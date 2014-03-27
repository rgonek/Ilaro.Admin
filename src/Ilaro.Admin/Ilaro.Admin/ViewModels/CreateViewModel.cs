using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.ViewModels
{
    public class CreateViewModel
    {
        public EntityViewModel Entity { get; set; }

        public IList<GroupPropertiesViewModel> PropertiesGroups { get; set; }
    }
}