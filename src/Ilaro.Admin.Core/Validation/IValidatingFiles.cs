using Ilaro.Admin.Core;

namespace Ilaro.Admin.Core.Validation
{
    public interface IValidatingFiles
    {
        bool Validate(PropertyValue propertyValue);
    }
}