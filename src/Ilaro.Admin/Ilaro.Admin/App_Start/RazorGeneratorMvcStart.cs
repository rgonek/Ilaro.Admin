using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using RazorGenerator.Mvc;
using Ilaro.Admin.Commons;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(Ilaro.Admin.App_Start.RazorGeneratorMvcStart), "Start")]

namespace Ilaro.Admin.App_Start
{
	public static class RazorGeneratorMvcStart
	{
		public static void Start()
		{
			var engine = new IlaroPrecompiledMvcEngine(typeof(RazorGeneratorMvcStart).Assembly)
			{
				AlwaysUsePhysicalViews = true
			};

			ViewEngines.Engines.Insert(0, engine);

			// StartPage lookups are done by WebPages. 
			VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);
		}
	}
}
