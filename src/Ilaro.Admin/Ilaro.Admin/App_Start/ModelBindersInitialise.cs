using Ilaro.Admin.App_Start;
using Ilaro.Admin.Core;
using Ilaro.Admin.Models;
using System.Web.Mvc;
using WebActivatorEx;

[assembly: PostApplicationStartMethod(typeof(ModelBindersInitialise), "Start")]
namespace Ilaro.Admin.App_Start
{
    public class ModelBindersInitialise
    {
        public static void Start()
        {
            var configuration = DependencyResolver.Current.GetService<IConfiguration>();
            ModelBinders.Binders.Add(typeof(TableInfo), new TableInfoModelBinder(configuration));
        }
    }
}