using Matchi.Application.Common.Interfaces;
using Matchi.Application.Workspaces;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Users.Queries;

public sealed record WorkspaceStateDto(
    IReadOnlyList<string> AvailableWorkspaces,
    string? PreferredWorkspace,
    string? LastWorkspace,
    string? ResolvedWorkspace);

public sealed record GetMyWorkspaceStateQuery : IRequest<WorkspaceStateDto>;

public sealed class GetMyWorkspaceStateQueryHandler : IRequestHandler<GetMyWorkspaceStateQuery, WorkspaceStateDto>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUserRepository _users;
    private readonly IWorkspaceAccessService _access;

    public GetMyWorkspaceStateQueryHandler(
        ICurrentUserService currentUser,
        IUserRepository users,
        IWorkspaceAccessService access)
    {
        _currentUser = currentUser;
        _users = users;
        _access = access;
    }

    public async Task<WorkspaceStateDto> Handle(GetMyWorkspaceStateQuery query, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Authenticated user was not found.");

        var user = await _users.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User was not found.");

        var available = await _access.ListAvailableAsync(userId, cancellationToken);
        var preferred = WorkspaceResolver.Normalize(user.PreferredWorkspace);
        var last = WorkspaceResolver.Normalize(user.LastWorkspace);

        return new WorkspaceStateDto(
            available,
            preferred,
            last,
            WorkspaceResolver.Resolve(available, preferred, last));
    }
}
