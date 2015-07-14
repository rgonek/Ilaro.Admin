using System;
using System.IO;
using System.Web.Mvc;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Areas.IlaroAdmin.Controllers
{
    public class ResourceController : Controller
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(ResourceController));
        private readonly Notificator _notificator;

        public ResourceController(Notificator notificator)
        {
            if (notificator == null)
                throw new ArgumentNullException("notificator");

            _notificator = notificator;
        }

        public virtual ActionResult Css(string id)
        {
            return GetResource("css", id);
        }

        public virtual ActionResult Script(string id)
        {
            return GetResource("script", id);
        }

        public virtual ActionResult Image(string id)
        {
            return GetResource("image", id);
        }

        public virtual ActionResult Fonts(string id)
        {
            return GetResource("fonts", id);
        }

        protected virtual ActionResult GetResource(string type, string file)
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
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            return HttpNotFound();
        }

        private Stream GetResourceStream(string folder, string file)
        {
            return GetType().Assembly
                .GetManifestResourceStream("Ilaro.Admin." + folder + "." + file);
        }
    }
}
