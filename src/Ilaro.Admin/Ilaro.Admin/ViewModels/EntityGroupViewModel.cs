using Ilaro.Admin.Core;
using System.Collections.Generic;

namespace Ilaro.Admin.ViewModels
{
    public class EntityGroupViewModel
    {
        public string Name { get; set; }

        public IList<Entity> Entities { get; set; }
    }
}