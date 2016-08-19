using Ilaro.Admin.Core.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Ilaro.Admin.Core.Validation
{
    public static class ValidationAttributeExtensions
    {
        public static ValidationAttribute SetErrorMessage(
            this ValidationAttribute attribute, 
            string errorMessage)
        {
            if (errorMessage.HasValue())
            {
                attribute.ErrorMessage = errorMessage;
            }

            return attribute;
        }
    }
}