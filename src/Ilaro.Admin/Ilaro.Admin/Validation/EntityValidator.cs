using System;
using System.ComponentModel.DataAnnotations;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Validation
{
    public class EntityValidator : IValidateEntity
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(EntityValidator));
        private readonly Notificator _notificator;
        private readonly IValidatingFiles _fileValidator;

        public EntityValidator(Notificator notificator, IValidatingFiles fileValidator)
        {
            if (notificator == null)
                throw new ArgumentNullException("notificator");
            if (fileValidator == null)
                throw new ArgumentNullException("fileValidator");

            _notificator = notificator;
            _fileValidator = fileValidator;
        }

        public bool Validate(Entity entity)
        {
            var instance = entity.CreateIntance();
            var context = new ValidationContext(instance);
            var isValid = true;
            foreach (var property in entity.Properties)
            {
                if (property.TypeInfo.IsFile)
                {
                    var result = _fileValidator.Validate(property);
                    if (result == false)
                        isValid = false;
                }
                foreach (var validator in property.ValidationAttributes)
                {
                    try
                    {
                        validator.Validate(property.Value.Raw, context);
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
    }
}