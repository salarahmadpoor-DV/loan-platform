using FluentValidation;
using FluentValidation.Results;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Users.Queries;
using Matchi.Application.Workspaces;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Users.Commands;

public sealed record SetLastWorkspaceCommand(string Workspace) : IRequest<WorkspaceStateDto>;

public sealed class SetLastWorkspaceCommandValidator : AbstractValidator<SetLastWorkspaceCommand>
{
    public SetLastWorkspaceCommandValidator()
    {
        RuleFor(x => x.Workspace).NotEmpty().MaximumLength(20);
    }
}

public sealed class SetLastWorkspaceCommandHandler : IRequestHandler<SetLastWorkspaceCommand, WorkspaceStateDto>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUserRepository _users;
    private readonly IWorkspaceAccessService _access;

    public SetLastWorkspaceCommandHandler(
        ICurrentUserService currentUser,
        IUserRepository users,
        IWorkspaceAccessService access)
    {
        _currentUser = currentUser;
        _users = users;
        _access = access;
    }

    public async Task<WorkspaceStateDto> Handle(SetLastWorkspaceCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Authenticated user was not found.");

        var workspace = WorkspaceResolver.Normalize(command.Workspace)
            ?? throw InvalidWorkspace();

        var user = await _users.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User was not found.");

        var available = await _access.ListAvailableAsync(userId, cancellationToken);
        if (!WorkspaceResolver.Contains(available, workspace))
            throw InvalidWorkspace();

        user.SetLastWorkspace(workspace);
        await _users.UpdateAsync(user, cancellationToken);

        var preferred = WorkspaceResolver.Normalize(user.PreferredWorkspace);
        return new WorkspaceStateDto(
            available,
            preferred,
            workspace,
            WorkspaceResolver.Resolve(available, preferred, workspace));
    }

    private static ValidationException InvalidWorkspace() =>
        new(new[] { new ValidationFailure("workspace", "This workspace is not available.") });
}
