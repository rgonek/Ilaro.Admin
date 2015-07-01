using System.Web.Mvc;
using System.Web.WebPages;
using Ilaro.Admin.App_Start;
using WebActivatorEx;

[assembly: PostApplicationStartMethod(typeof(RazorGeneratorMvcStart), "Start")]
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
