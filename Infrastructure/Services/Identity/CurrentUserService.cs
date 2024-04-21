using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Identity;

public class CurrentUserService : ICurrentUserService
{
    public string UserId { get; }

    public CurrentUserService(IHttpContextAccessor httpContext)
    {
        UserId = httpContext.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
    }
}
