using Ilaro.Admin.Core;

namespace Ilaro.Admin.Validation
{
    public interface IValidatingEntities
    {
        bool Validate(EntityRecord entityRecord);
    }
}