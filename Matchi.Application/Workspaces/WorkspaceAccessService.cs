using Matchi.Domain.Interfaces;

namespace Matchi.Application.Workspaces;

public sealed class WorkspaceAccessService : IWorkspaceAccessService
{
    private readonly IUserRoleRepository _roles;
    private readonly IProviderRepository _providers;
    private readonly IBusinessRepository _businesses;

    public WorkspaceAccessService(
        IUserRoleRepository roles,
        IProviderRepository providers,
        IBusinessRepository businesses)
    {
        _roles = roles;
        _providers = providers;
        _businesses = businesses;
    }

    public async Task<IReadOnlyList<string>> ListAvailableAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        var roles = await _roles.GetRoleCodesByUserIdAsync(userId, cancellationToken);
        var codes = roles
            .Select(role => role.Trim().ToUpperInvariant())
            .ToHashSet(StringComparer.Ordinal);

        if (codes.Contains("ADMIN"))
            return WorkspaceIds.FallbackOrder;

        var hasCustomer = codes.Contains("USER");
        var provider = await _providers.GetByUserIdAsync(userId, cancellationToken);
        var owned = await _businesses.GetByOwnerUserIdAsync(userId, cancellationToken);

        return WorkspaceResolver.Available(
            hasCustomer,
            provider is not null,
            owned.Count > 0);
    }
}
