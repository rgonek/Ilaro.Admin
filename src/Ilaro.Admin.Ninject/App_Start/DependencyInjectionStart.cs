using Ilaro.Admin.Ninject.App_Start;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(DependencyInjectionStart), "Start")]
[assembly: ApplicationShutdownMethod(typeof(DependencyInjectionStart), "Stop")]
namespace Ilaro.Admin.Ninject.App_Start
{
    public static class DependencyInjectionStart
    {
        public static void Start()
        {
            Bootstrapper.Initialise();
        }

        public static void Stop()
        {
            Bootstrapper.ShutDown();
        }
    }
}
