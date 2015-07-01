using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.FileUpload;
using DataType = Ilaro.Admin.Core.DataType;

namespace Ilaro.Admin.Validation
{
    public class EntityValidator : IValidateEntity
    {
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
                    //modelState.AddModelError(property.Name, IlaroAdminResources.UnvalidFile);
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
                    catch (ValidationException exc)
                    {
                        isValid = false;
                        //modelState.AddModelError(property.Name, exc.Message);
                    }
                }
            }
            return isValid;
        }
    }
}