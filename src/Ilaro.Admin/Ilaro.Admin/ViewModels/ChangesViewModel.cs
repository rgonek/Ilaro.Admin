using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.EntitiesFilters;

namespace Ilaro.Admin.ViewModels
{
	public class ChangesViewModel : DetailsViewModel
	{
		public EntityViewModel EntityChangesFor { get; set; }
	}
}