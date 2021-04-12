using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ilaro.Admin.Core.Validation
{
    public class FileValidator : IValidatingFiles
    {
        //private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(EntityValidator));
        private readonly INotificator _notificator;
        private readonly IAppConfiguration _configuration;

        public FileValidator(INotificator notificator, IAppConfiguration configuration)
        {
            if (notificator == null)
                throw new ArgumentNullException(nameof(notificator));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _notificator = notificator;
            _configuration = configuration;
        }

        public bool Validate(PropertyValue propertyValue)
        {
            var file = (IFormFile)propertyValue.Raw;
            if (file == null || file.Length == 0)
            {
                if (propertyValue.Property.IsRequired)
                {
                    _notificator.AddModelError(propertyValue.Property.Name, "Missing file.");
                    return false;
                }
                return true;
            }

            var maxFileSize = propertyValue.Property.FileOptions.MaxFileSize.GetValueOrDefault(_configuration.MaxFileSize);
            if (file.Length > maxFileSize)
            {
                _notificator.AddModelError(propertyValue.Property.Name, "File is too big.");
                return false;
            }

            if (propertyValue.Property.TypeInfo.IsImage && IsImage(file) == false)
            {
                _notificator.AddModelError(propertyValue.Property.Name, "File is not a image.");
                return false;
            }

            var allowedFileExtensions = propertyValue.Property.FileOptions.AllowedFileExtensions;
            if (allowedFileExtensions == null || allowedFileExtensions.Length == 0)
            {
                allowedFileExtensions = _configuration.AllowedFileExtensions;
            }
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (allowedFileExtensions.Contains(ext) == false)
            {
                _notificator.AddModelError(propertyValue.Property.Name, "Wrong file extension.");
                return false;
            }

            return true;
        }

        private static bool IsImage(IFormFile file)
        {
            return Regex.IsMatch(file.ContentType, "image/\\S+");
        }
    }
}
