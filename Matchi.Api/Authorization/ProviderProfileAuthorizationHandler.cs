using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Matchi.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Matchi.Api.Authorization;

/// <summary>
/// Succeeds when the authenticated user owns a Provider row (<c>Providers.UserId</c>)
/// or has the ADMIN role. Does not use a JWT <c>PROVIDER</c> registration role.
/// </summary>
public sealed class ProviderProfileAuthorizationHandler
    : AuthorizationHandler<ProviderProfileRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProviderRepository _providers;

    public ProviderProfileAuthorizationHandler(
        IHttpContextAccessor httpContextAccessor,
        IProviderRepository providers)
    {
        _httpContextAccessor = httpContextAccessor;
        _providers = providers;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ProviderProfileRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return;

        if (context.User.IsInRole("ADMIN"))
        {
            context.Succeed(requirement);
            return;
        }

        var userIdValue = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!long.TryParse(userIdValue, out var userId))
            return;

        var cancellation = _httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;
        var provider = await _providers.GetByUserIdAsync(userId, cancellation);
        if (provider is not null)
            context.Succeed(requirement);
    }
}
