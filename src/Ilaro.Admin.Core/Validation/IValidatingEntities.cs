using Ilaro.Admin.Core;

namespace Ilaro.Admin.Core.Validation
{
    public interface IValidatingEntities
    {
        bool Validate(EntityRecord entityRecord);
    }
}