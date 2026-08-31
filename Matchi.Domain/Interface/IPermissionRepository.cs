public interface IPermissionRepository
{
    Task<IReadOnlyList<string>> GetPermissionCodesByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default);
}