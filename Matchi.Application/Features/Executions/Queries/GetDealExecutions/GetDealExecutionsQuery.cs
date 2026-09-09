using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Executions;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Executions.Queries.GetDealExecutions;

public sealed record GetDealExecutionsQuery(long DealId) : IRequest<IReadOnlyList<ServiceExecutionDto>>;

public sealed class GetDealExecutionsQueryHandler
    : IRequestHandler<GetDealExecutionsQuery, IReadOnlyList<ServiceExecutionDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IServiceExecutionRepository _executions;

    public GetDealExecutionsQueryHandler(
        ICurrentUserService currentUserService,
        IServiceExecutionRepository executions)
    {
        _currentUserService = currentUserService;
        _executions = executions;
    }

    public async Task<IReadOnlyList<ServiceExecutionDto>> Handle(
        GetDealExecutionsQuery query,
        CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        if (!await _executions.IsDealVisibleAsync(query.DealId, userId, cancellationToken))
            throw new KeyNotFoundException("Deal was not found.");

        var items = await _executions.ListVisibleByDealAsync(query.DealId, userId, cancellationToken);
        return items.Select(ExecutionDtoMapper.ToDto).ToList();
    }
}
