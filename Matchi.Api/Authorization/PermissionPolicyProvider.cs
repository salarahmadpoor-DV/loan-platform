using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Matchi.Api.Authorization;

/// <summary>
/// Unknown policy names are treated as JWT <c>permission</c> claims (existing Matchi model).
/// Named non-permission policies must be resolved here; otherwise
/// <c>ProviderWorkspace</c> is mistaken for permission <c>ProviderWorkspace</c> and always 403s.
/// </summary>
public sealed class PermissionPolicyProvider
    : DefaultAuthorizationPolicyProvider
{
    public PermissionPolicyProvider(
        IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(
        string policyName)
    {
        if (string.Equals(policyName, "ProviderWorkspace", StringComparison.Ordinal))
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new ProviderProfileRequirement())
                .Build();
        }

        var policy = await base.GetPolicyAsync(policyName);

        if (policy != null)
            return policy;

        return new AuthorizationPolicyBuilder()
            .AddRequirements(
                new PermissionRequirement(policyName))
            .Build();
    }
}
