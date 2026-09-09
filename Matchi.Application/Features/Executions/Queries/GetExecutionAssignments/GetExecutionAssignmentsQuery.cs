using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Executions;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Executions.Queries.GetExecutionAssignments;

public sealed record GetExecutionAssignmentsQuery(long ExecutionId) : IRequest<IReadOnlyList<ExecutionAssignmentDto>>;

public sealed class GetExecutionAssignmentsQueryHandler
    : IRequestHandler<GetExecutionAssignmentsQuery, IReadOnlyList<ExecutionAssignmentDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IServiceExecutionRepository _executions;
    private readonly IExecutionAssignmentRepository _assignments;

    public GetExecutionAssignmentsQueryHandler(
        ICurrentUserService currentUserService,
        IServiceExecutionRepository executions,
        IExecutionAssignmentRepository assignments)
    {
        _currentUserService = currentUserService;
        _executions = executions;
        _assignments = assignments;
    }

    public async Task<IReadOnlyList<ExecutionAssignmentDto>> Handle(
        GetExecutionAssignmentsQuery query,
        CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        var execution = await _executions.GetVisibleByIdAsync(query.ExecutionId, userId, cancellationToken);
        if (execution is null)
            throw new KeyNotFoundException("Execution was not found.");

        var items = await _assignments.ListVisibleAsync(query.ExecutionId, userId, cancellationToken);
        return items.Select(ExecutionDtoMapper.ToDto).ToList();
    }
}
