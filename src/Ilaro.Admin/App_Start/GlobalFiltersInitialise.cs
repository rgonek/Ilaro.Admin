using Ilaro.Admin.App_Start;
using Ilaro.Admin.Infrastructure;
using System.Web.Mvc;
using WebActivatorEx;

[assembly: PostApplicationStartMethod(typeof(GlobalFiltersInitialise), "Start")]
namespace Ilaro.Admin.App_Start
{
    public class GlobalFiltersInitialise
    {
        public static void Start()
        {
            GlobalFilters.Filters.Add(new CopyIsAjaxRequestFromRequestToViewBagAttribute());
            GlobalFilters.Filters.Add(new ModelStateErrorsBuilderAttribute());
            GlobalFilters.Filters.Add(new NotificatorAttribute());
        }
    }
}