namespace Matchi.Application.Workspaces;

public interface IWorkspaceAccessService
{
    Task<IReadOnlyList<string>> ListAvailableAsync(long userId, CancellationToken cancellationToken = default);
}
