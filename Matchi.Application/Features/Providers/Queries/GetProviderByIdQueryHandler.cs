using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Providers.Queries;

public sealed class GetProviderByIdQueryHandler : IRequestHandler<GetProviderByIdQuery, ProviderDto?>
{
    private readonly IProviderRepository _providerRepository;

    public GetProviderByIdQueryHandler(IProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
    }

    public async Task<ProviderDto?> Handle(
        GetProviderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var provider = await _providerRepository.GetByIdAsync(request.ProviderId, cancellationToken);
        if (provider is null)
            return null;

        return ProviderDto.From(provider);
    }
}
