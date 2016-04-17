using Ilaro.Admin.Autofac.App_Start;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(DependencyInjectionStart), "Start")]
namespace Ilaro.Admin.Autofac.App_Start
{
    public static class DependencyInjectionStart
    {
        public static void Start()
        {
            Bootstrapper.Initialise();
        }
    }
}
