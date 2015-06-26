using Ilaro.Admin.Unity.App_Start;
using WebActivatorEx;

[assembly: PostApplicationStartMethod(typeof(DependencyInjectionStart), "Start")]

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
