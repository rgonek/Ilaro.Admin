using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.ViewModels
{
    public class EditViewModel
    {
        public EntityViewModel Entity { get; set; }

        public IList<GroupPropertiesViewModel> PropertiesGroups { get; set; }
    }
}