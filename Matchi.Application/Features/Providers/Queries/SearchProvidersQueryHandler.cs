using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Providers.Queries;

public sealed class SearchProvidersQueryHandler : IRequestHandler<SearchProvidersQuery, IEnumerable<ProviderDto>>
{
    private readonly IProviderRepository _providerRepository;

    public SearchProvidersQueryHandler(IProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
    }

    public async Task<IEnumerable<ProviderDto>> Handle(
        SearchProvidersQuery request,
        CancellationToken cancellationToken)
    {
        var providers = request.ServiceId.HasValue
            ? await _providerRepository.GetProvidersByServiceIdAsync(request.ServiceId.Value, cancellationToken)
            : Array.Empty<Matchi.Domain.Entities.Provider>();

        return providers.Select(x => new ProviderDto(x.Id, x.Name, x.Rating, x.IsActive, x.Lat, x.Lng));
    }
}
