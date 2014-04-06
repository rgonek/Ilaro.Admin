using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ilaro.Web.Sample.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return RedirectToAction("Index", "IlaroAdmin");
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your app description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}
