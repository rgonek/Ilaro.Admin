using Ilaro.Admin.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Ilaro.Admin.Validation
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