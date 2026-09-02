namespace Matchi.Domain.Interfaces;

public interface IUserRoleRepository
{
    Task EnsureRoleAsync(
        long userId,
        string roleCode,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRoleCodesByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetPermissionCodesByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default);
}