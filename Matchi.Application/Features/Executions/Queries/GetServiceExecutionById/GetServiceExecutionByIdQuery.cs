using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Executions;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Executions.Queries.GetServiceExecutionById;

public sealed record GetServiceExecutionByIdQuery(long ExecutionId) : IRequest<ServiceExecutionDto?>;

public sealed class GetServiceExecutionByIdQueryHandler
    : IRequestHandler<GetServiceExecutionByIdQuery, ServiceExecutionDto?>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IServiceExecutionRepository _executions;

    public GetServiceExecutionByIdQueryHandler(
        ICurrentUserService currentUserService,
        IServiceExecutionRepository executions)
    {
        _currentUserService = currentUserService;
        _executions = executions;
    }

    public async Task<ServiceExecutionDto?> Handle(
        GetServiceExecutionByIdQuery query,
        CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        var execution = await _executions.GetVisibleByIdAsync(query.ExecutionId, userId, cancellationToken);
        return execution is null ? null : ExecutionDtoMapper.ToDto(execution);
    }
}
