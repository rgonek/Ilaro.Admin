using System;
using System.Web;
using System.Web.Mvc;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Core.File;
using Ilaro.Admin.Ninject.App_Start;
using WebActivatorEx;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(DependencyInjectionStart), "Start")]
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
