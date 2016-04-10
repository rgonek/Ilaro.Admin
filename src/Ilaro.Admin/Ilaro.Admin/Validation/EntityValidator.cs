using System;
using System.ComponentModel.DataAnnotations;
using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Validation
{
    public class EntityValidator : IValidatingEntities
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(EntityValidator));
        private readonly Notificator _notificator;
        private readonly IValidatingFiles _fileValidator;

        public EntityValidator(Notificator notificator, IValidatingFiles fileValidator)
        {
            if (notificator == null)
                throw new ArgumentNullException(nameof(notificator));
            if (fileValidator == null)
                throw new ArgumentNullException(nameof(fileValidator));

            _notificator = notificator;
            _fileValidator = fileValidator;
        }

        public bool Validate(EntityRecord entityRecord)
        {
            var instance = entityRecord.CreateInstance();
            var context = new ValidationContext(instance);
            var isValid = true;
            foreach (var propertyValue in entityRecord.Values)
            {
                if (propertyValue.Property.TypeInfo.IsFile)
                {
                    var result = _fileValidator.Validate(propertyValue);
                    if (result == false)
                        isValid = false;
                }

                context.DisplayName = propertyValue.Property.Display;
                foreach (var validator in propertyValue.Property.Validators)
                {
                    try
                    {
                        validator.Validate(propertyValue.Raw, context);
                    }
                    catch (ValidationException ex)
                    {
                        isValid = false;
                        _notificator.AddModelError(propertyValue.Property.Name, ex.Message);
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