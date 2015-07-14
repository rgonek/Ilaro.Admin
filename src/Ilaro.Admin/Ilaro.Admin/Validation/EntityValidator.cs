using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Ilaro.Admin.Core;
using DataType = Ilaro.Admin.Core.DataType;

namespace Ilaro.Admin.Validation
{
    public class EntityValidator : IValidateEntity
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(EntityValidator));
        private readonly Notificator _notificator;
        private readonly IConfiguration _configuration;

        public EntityValidator(Notificator notificator, IConfiguration configuration)
        {
            if (notificator == null)
                throw new ArgumentNullException("notificator");
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            _notificator = notificator;
            _configuration = configuration;
        }

        public bool Validate(Entity entity)
        {
            var isValid = true;
            foreach (var property in entity.Properties.Where(x => x.TypeInfo.DataType == DataType.File))
            {
                var file = (HttpPostedFile)property.Value.Raw;
                var allowedFileExtensions = property.FileOptions.AllowedFileExtensions;
                if (allowedFileExtensions == null || allowedFileExtensions.Length == 0)
                {
                    allowedFileExtensions = _configuration.AllowedFileExtensions;
                }
                //var result = FileUpload.Validate(
                //    file,
                //    property.ImageOptions.MaxFileSize.GetValueOrDefault(_configuration.MaxFileSize),
                //    allowedFileExtensions,
                //    !property.IsRequired);

                //if (result != FileUploadValidationResult.Valid)
                //{
                //    isValid = false;
                //    // TODO: more complex validation message
                //    _notificator.AddModelError(property.Name, IlaroAdminResources.UnvalidFile);
                //}
            }

            foreach (var property in entity.Properties.Where(x => x.TypeInfo.DataType != DataType.File))
            {
                foreach (var validator in property.ValidationAttributes)
                {
                    try
                    {
                        validator.Validate(property.Value.Raw, property.Name);
                    }
                    catch (ValidationException ex)
                    {
                        isValid = false;
                        _notificator.AddModelError(property.Name, ex.Message);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                    }
                }
            }
            return isValid;
        }

        //public static FileUploadValidationResult Validate(
        //    HttpPostedFile file,
        //    long maxFileSize,
        //    string[] allowedFileExtensions,
        //    bool allowEmptyFile = true)
        //{
        //    if (file == null || file.ContentLength <= 0)
        //    {
        //        return allowEmptyFile ?
        //            FileUploadValidationResult.Valid :
        //            FileUploadValidationResult.EmptyFile;
        //    }

        //    if (file.ContentLength > maxFileSize)
        //    {
        //        return FileUploadValidationResult.TooBigFile;
        //    }

        //    var ext = Path.GetExtension(file.FileName).ToLower();

        //    if (allowedFileExtensions.Contains(ext))
        //    {
        //        return FileUploadValidationResult.Valid;
        //    }

        //    if (!IsImage(file))
        //    {
        //        return FileUploadValidationResult.NotImage;
        //    }

        //    return FileUploadValidationResult.WrongExtension;
        //}

        //private static bool IsImage(HttpPostedFile file)
        //{
        //    return Regex.IsMatch(file.ContentType, "image/\\S+");
        //}

        //private static string GetErrorMessage(
        //    FileUploadValidationResult validationResult,
        //    params string[] allowedFileExtensions)
        //{
        //    switch (validationResult)
        //    {
        //        case FileUploadValidationResult.EmptyFile:
        //            return "You must choose a file.";
        //        case FileUploadValidationResult.TooBigFile:
        //            return "File is to big. Max file size is 2MB.";
        //        case FileUploadValidationResult.WrongExtension:
        //            return
        //                "Uploaded file has illegal extension. Available exetonsions: " +
        //                string.Join(", ", allowedFileExtensions);
        //        case FileUploadValidationResult.NotImage:
        //            return "Uploaded file isn't a image.";
        //    }

        //    return String.Empty;
        //}
        //public enum FileUploadValidationResult
        //{
        //    EmptyFile,
        //    TooBigFile,
        //    WrongExtension,
        //    NotImage,
        //    Valid
        //}
    }
}