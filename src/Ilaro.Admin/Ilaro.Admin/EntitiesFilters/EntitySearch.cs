using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.ViewModels;

namespace Ilaro.Admin.EntitiesFilters
{
    public class EntitySearch
    {
        public string Query { get; set; }

        public IEnumerable<PropertyViewModel> Properties { get; set; }

        public bool IsActive
        {
            get
            {
                return !Query.IsNullOrEmpty() && Properties.Any();
            }
        }
    }
}