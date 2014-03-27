using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.ViewModels
{
    public class EntityGroupViewModel
    {
        public string Name { get; set; }

        public IList<EntityViewModel> Entities { get; set; }
    }
}