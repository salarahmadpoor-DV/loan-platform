using Matchi.Application.Common;
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
        if (!request.ServiceId.HasValue)
            return Array.Empty<ProviderDto>();

        var paging = ListPaging.Normalize(request.Page, request.PageSize);
        var providers = await _providerRepository.GetProvidersByServiceIdAsync(
            request.ServiceId.Value,
            paging.Skip,
            paging.PageSize,
            cancellationToken);

        return providers.Select(ProviderDto.From);
    }
}
