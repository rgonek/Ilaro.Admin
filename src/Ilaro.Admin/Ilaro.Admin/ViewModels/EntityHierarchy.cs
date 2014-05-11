using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.ViewModels
{
	public class EntityHierarchy
	{
		public EntityViewModel Entity { get; set; }

		public string Alias { get; set; }

		public IList<EntityHierarchy> SubHierarchies { get; set; }

		public EntityHierarchy ParentHierarchy { get; set; }
	}
}