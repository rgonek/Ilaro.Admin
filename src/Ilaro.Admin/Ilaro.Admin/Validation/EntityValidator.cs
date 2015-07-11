using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.FileUpload;
using Resources;
using DataType = Ilaro.Admin.Core.DataType;

namespace Ilaro.Admin.Validation
{
    public class EntityValidator : IValidateEntity
    {
        private readonly Notificator _notificator;

        public EntityValidator(Notificator notificator)
        {
            if (notificator == null)
                throw new ArgumentNullException("notificator");

            _notificator = notificator;
        }

        public bool Validate(Entity entity)
        {
            var isValid = true;
            foreach (var property in entity.Properties.Where(x => x.TypeInfo.DataType == DataType.File))
            {
                var file = (HttpPostedFile)property.Value.Raw;
                var result = FileUpload.Validate(
                    file,
                    property.ImageOptions.MaxFileSize,
                    property.ImageOptions.AllowedFileExtensions,
                    !property.IsRequired);

                if (result != FileUploadValidationResult.Valid)
                {
                    isValid = false;
                    // TODO: more complex validation message
                    _notificator.AddModelError(property.Name, IlaroAdminResources.UnvalidFile);
                }
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
                }
            }
            return isValid;
        }
    }
}