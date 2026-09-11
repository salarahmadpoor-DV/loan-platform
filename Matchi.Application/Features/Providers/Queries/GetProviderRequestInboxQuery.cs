using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Matching;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Providers.Queries;

public sealed record GetProviderRequestInboxQuery : IRequest<IReadOnlyList<ProviderRequestInboxItemDto>>;

public sealed class GetProviderRequestInboxQueryHandler
    : IRequestHandler<GetProviderRequestInboxQuery, IReadOnlyList<ProviderRequestInboxItemDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;
    private readonly IMatchingReadRepository _matching;

    public GetProviderRequestInboxQueryHandler(
        ICurrentUserService currentUserService,
        IProviderRepository providers,
        IMatchingReadRepository matching)
    {
        _currentUserService = currentUserService;
        _providers = providers;
        _matching = matching;
    }

    public async Task<IReadOnlyList<ProviderRequestInboxItemDto>> Handle(
        GetProviderRequestInboxQuery query,
        CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        var requests = await _matching.ListOpenEligibleRequestsForProviderAsync(
            provider.Id,
            cancellationToken);

        return requests.Select(ToInboxItem).ToList();
    }

    internal static ProviderRequestInboxItemDto ToInboxItem(Request request)
    {
        var services = request.Services.Where(s => !s.IsDeleted).OrderBy(s => s.DisplayOrder).ThenBy(s => s.Id).ToList();
        var products = request.Products.Where(p => !p.IsDeleted).OrderBy(p => p.DisplayOrder).ThenBy(p => p.Id).ToList();
        var location = request.Locations.OrderBy(l => l.Id).FirstOrDefault();

        var serviceNames = services
            .Select(s => s.Service?.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct()
            .ToList();

        var categoryNames = services
            .Select(s => s.Service?.Category?.Name)
            .Concat(products.Select(p => p.ProductCategory?.Name))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct()
            .ToList();

        ProviderInboxLocationDto? locationDto = null;
        if (location is not null
            && (!string.IsNullOrWhiteSpace(location.Province)
                || !string.IsNullOrWhiteSpace(location.City)
                || !string.IsNullOrWhiteSpace(location.District)))
        {
            locationDto = new ProviderInboxLocationDto(location.Province, location.City, location.District);
        }

        return new ProviderRequestInboxItemDto(
            request.Id,
            request.RequestType,
            serviceNames.Count == 0 ? null : string.Join(", ", serviceNames),
            categoryNames.Count == 0 ? null : string.Join(", ", categoryNames),
            locationDto,
            request.CreateDate,
            request.Status);
    }
}
