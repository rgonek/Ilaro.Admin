using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.ViewModels
{
    public class GroupPropertiesViewModel
    {
        public string GroupName { get; set; }

        public IList<PropertyViewModel> Properties { get; set; }

        public bool IsCollapsed { get; set; }
    }
}