using Ilaro.Admin.Core;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Ilaro.Admin.Validation
{
    public class FileValidator : IValidatingFiles
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(EntityValidator));
        private readonly Notificator _notificator;
        private readonly IConfiguration _configuration;

        public FileValidator(Notificator notificator, IConfiguration configuration)
        {
            if (notificator == null)
                throw new ArgumentNullException(nameof(notificator));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _notificator = notificator;
            _configuration = configuration;
        }

        public bool Validate(Property property)
        {
            var file = (HttpPostedFile)property.Value.Raw;
            if (file == null)
            {
                _notificator.AddModelError(property.Name, "Missing file.");
                return false;
            }

            var maxFileSize = property.FileOptions.MaxFileSize.GetValueOrDefault(_configuration.MaxFileSize);
            if (file.ContentLength > maxFileSize)
            {
                _notificator.AddModelError(property.Name, "File is too big.");
                return false;
            }

            if (property.TypeInfo.IsImage && IsImage(file) == false)
            {
                _notificator.AddModelError(property.Name, "File is not a image.");
                return false;
            }

            var allowedFileExtensions = property.FileOptions.AllowedFileExtensions;
            if (allowedFileExtensions == null || allowedFileExtensions.Length == 0)
            {
                allowedFileExtensions = _configuration.AllowedFileExtensions;
            }
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (allowedFileExtensions.Contains(ext))
            {
                _notificator.AddModelError(property.Name, "Wrong file extension.");
                return false;
            }

            return true;
        }

        private static bool IsImage(HttpPostedFile file)
        {
            return Regex.IsMatch(file.ContentType, "image/\\S+");
        }
    }
}