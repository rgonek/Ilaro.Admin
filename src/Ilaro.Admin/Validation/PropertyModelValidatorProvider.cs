using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Ilaro.Admin.Validation
{
    public class PropertyModelValidatorProvider : DataAnnotationsModelValidatorProvider
    {
        public IEnumerable<ModelValidator> GetValidatorsForAttributes(
            ModelMetadata metadata,
            ControllerContext context,
            IEnumerable<Attribute> attributes)
        {
            return GetValidators(metadata, context, attributes);
        }
    }
}