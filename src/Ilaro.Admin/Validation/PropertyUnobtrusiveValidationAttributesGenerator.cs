using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Ilaro.Admin.Validation
{
    public static class PropertyUnobtrusiveValidationAttributesGenerator
    {
        public static IDictionary<string, object> GetValidationAttributes(
            Property property,
            ControllerContext context)
        {
            // create own metadata based on Property object
            var metadata = new ModelMetadata(
                ModelMetadataProviders.Current,
                property.Entity.Type,
                null,
                property.TypeInfo.Type,
                property.Name);
            metadata.DisplayName = property.Display;

            var clientRules = ModelValidatorProviders.Providers
                .GetValidators(metadata, context)
                .SelectMany(v => v.GetClientValidationRules());

            var provider = new PropertyModelValidatorProvider();
            var modelValidators = provider
                .GetValidatorsForAttributes(metadata, context, property.Validators);
            clientRules = clientRules
                .Union(modelValidators.SelectMany(x => x.GetClientValidationRules()))
                .DistinctBy(x => x.ValidationType);

            var validationAttributes = new Dictionary<string, object>();
            UnobtrusiveValidationAttributesGenerator
                .GetValidationAttributes(clientRules, validationAttributes);

            return validationAttributes;
        }
    }
}