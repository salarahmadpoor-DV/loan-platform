using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Providers.Queries;

public sealed record GetMyProviderExecutionsQuery : IRequest<IReadOnlyList<ProviderExecutionDto>>;

public sealed class GetMyProviderExecutionsQueryHandler
    : IRequestHandler<GetMyProviderExecutionsQuery, IReadOnlyList<ProviderExecutionDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;
    private readonly IServiceExecutionRepository _executions;

    public GetMyProviderExecutionsQueryHandler(
        ICurrentUserService currentUserService,
        IProviderRepository providers,
        IServiceExecutionRepository executions)
    {
        _currentUserService = currentUserService;
        _providers = providers;
        _executions = executions;
    }

    public async Task<IReadOnlyList<ProviderExecutionDto>> Handle(
        GetMyProviderExecutionsQuery query,
        CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        var executions = await _executions.ListVisibleToProviderAsync(provider.Id, cancellationToken);

        return executions
            .Select(execution => new ProviderExecutionDto(
                execution.Id,
                execution.DealId,
                execution.BusinessId,
                execution.Status,
                execution.ScheduledDate,
                execution.ScheduledTimeFrom,
                execution.ScheduledTimeTo,
                execution.StartedAt,
                execution.CompletedAt))
            .ToList();
    }
}
