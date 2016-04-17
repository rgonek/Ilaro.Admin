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
                throw new ArgumentNullException(nameof(notificator));

            _notificator = notificator;
        }

        public virtual ActionResult Index(string file)
        {
            return GetResource(file);
        }

        protected virtual ActionResult GetResource(string file)
        {
            if (string.IsNullOrEmpty(file))
                return HttpNotFound();

            file = file.Replace("_", ".");
            var extension = Path.GetExtension(file);
            string contentType, folder;
            switch (extension.ToLower())
            {
                case ".js":
                    contentType = "text/javascript";
                    folder = "Scripts";
                    break;
                case ".css":
                    contentType = "text/css";
                    folder = "Content.css";
                    break;
                case ".gif":
                case ".jpg":
                case ".png":
                    contentType = "image/" + extension.TrimStart('.');
                    folder = "Content.img";
                    break;
                default:
                    contentType = "";
                    folder = "Content.fonts";
                    break;
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
