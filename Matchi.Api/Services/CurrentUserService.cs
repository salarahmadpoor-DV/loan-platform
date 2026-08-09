using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Matchi.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Matchi.Api.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public long? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var claimValue = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return long.TryParse(claimValue, out var userId)
                ? userId
                : null;
        }
    }

    public string? Mobile =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.MobilePhone);
}
