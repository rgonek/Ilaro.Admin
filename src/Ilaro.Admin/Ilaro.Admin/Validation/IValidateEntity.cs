using Ilaro.Admin.Core;

namespace Ilaro.Admin.Validation
{
    public interface IValidateEntity
    {
        bool Validate(Entity entity);
    }
}