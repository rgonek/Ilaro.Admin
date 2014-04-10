using Ilaro.Admin.Commons.Notificator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ilaro.Admin.Controllers
{
	public class IlaroAdminResourceController : BaseController
	{
		public IlaroAdminResourceController(Notificator notificator)
			: base(notificator)
		{
		}

		public ActionResult Css(string id)
		{
			return GetResource("css", id);
		}

		public ActionResult Script(string id)
		{
			return GetResource("script", id);
		}

		public ActionResult Image(string id)
		{
			return GetResource("image", id);
		}

		public ActionResult Fonts(string id)
		{
			return GetResource("fonts", id);
		}

		private ActionResult GetResource(string type, string file)
		{
			if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(file))
			{
				return HttpNotFound();
			}

			file = file.Replace("_", ".");

			string contentType, folder;

			switch (type.ToUpperInvariant())
			{
				case "SCRIPT":
					contentType = "text/javascript";
					folder = "Scripts";
					break;
				case "CSS":
					contentType = "text/css";
					folder = "Content.css";
					break;
				case "IMAGE":
					contentType = "image/" + Path.GetExtension(file).TrimStart('.');
					folder = "Content.img";
					break;
				case "FONTS":
					contentType = "";
					folder = "Content.fonts";
					break;
				default:
					return HttpNotFound();
			}

			try
			{
				using (var stream = GetResourceStream(folder, file))
				{
					stream.CopyTo(Response.OutputStream);
				}

				return Content(null, contentType);
			}
			catch { /* log */ }

			return HttpNotFound();
		}

		private Stream GetResourceStream(string folder, string file)
		{
			return this.GetType().Assembly.GetManifestResourceStream("Ilaro.Admin." + folder + "." + file);
		}
	}
}