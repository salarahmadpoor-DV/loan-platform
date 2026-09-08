using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Providers.Queries;

public sealed record GetMyProviderQuery : IRequest<ProviderProfileDto?>;

public sealed class GetMyProviderQueryHandler : IRequestHandler<GetMyProviderQuery, ProviderProfileDto?>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public GetMyProviderQueryHandler(
        ICurrentUserService currentUserService,
        IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<ProviderProfileDto?> Handle(GetMyProviderQuery query, CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        var provider = await _providers.GetByUserIdAsync(userId, cancellationToken);
        if (provider is null)
            return null;

        return new ProviderProfileDto(
            provider.Id,
            provider.UserId,
            provider.Name,
            provider.Mobile,
            provider.Description,
            provider.Lat is null ? null : (double)provider.Lat.Value,
            provider.Lng is null ? null : (double)provider.Lng.Value,
            provider.Rating,
            provider.ReviewCount,
            provider.CompletedJobCount,
            provider.Status);
    }
}

public sealed record GetMyProviderBusinessesQuery : IRequest<IReadOnlyList<ProviderMembershipDto>>;

public sealed class GetMyProviderBusinessesQueryHandler
    : IRequestHandler<GetMyProviderBusinessesQuery, IReadOnlyList<ProviderMembershipDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public GetMyProviderBusinessesQueryHandler(
        ICurrentUserService currentUserService,
        IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<IReadOnlyList<ProviderMembershipDto>> Handle(
        GetMyProviderBusinessesQuery query,
        CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        var memberships = await _providers.ListMembershipsAsync(provider.Id, cancellationToken);
        return memberships
            .Select(m => new ProviderMembershipDto(
                m.BusinessId,
                m.Business.Name,
                m.Role,
                m.Status,
                m.JoinedAt,
                m.LeftAt))
            .ToList();
    }
}
