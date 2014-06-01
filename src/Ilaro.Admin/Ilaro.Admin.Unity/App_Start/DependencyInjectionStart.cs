using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(Ilaro.Admin.Unity.App_Start.DependencyInjectionStart), "Start")]

namespace Ilaro.Admin.Unity.App_Start
{
	public static class DependencyInjectionStart
    {
        public static void Start()
        {
			Bootstrapper.Initialise();
        }
    }
}
