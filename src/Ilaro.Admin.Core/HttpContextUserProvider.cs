using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ilaro.Admin.Core
{
    public class HttpContextUserProvider : IUser
    {

        private readonly IHttpContextAccessor _accessor;

        public HttpContextUserProvider(IHttpContextAccessor accessor)
            => _accessor = accessor;

        public string UserName() => _accessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;

        public object Id() => _accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
    }
}