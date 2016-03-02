using Ilaro.Admin.Sample.DatabaseReset;
using NLog;
using System.Configuration;
using System.Web.Mvc;

namespace Ilaro.Admin.Sample.Controllers
{
    public class MaintenanceController : Controller
    {
        static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public ActionResult DatabaseReset(string token)
        {
            var token_to_compare = ConfigurationManager.AppSettings["DatabaseResetToken"];

            if (token != token_to_compare)
            {
                _log.Warn("Wrong token was provided for DatabaseReset ({0})", token);

                return new HttpStatusCodeResult(400, "Wrong token.");
            }

            DatabaseResetJob.Execute();
            return Content("Ok");
        }
    }
}
