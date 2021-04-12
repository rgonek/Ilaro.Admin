using System.Security.Claims;

namespace Ilaro.Admin.Authorization
{
    public interface IProvideClaimsPrincipal
    {
        ClaimsPrincipal User { get; }
    }
}
