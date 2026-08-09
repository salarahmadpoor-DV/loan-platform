using MediatR;

namespace Matchi.Application.Features.Providers.Queries;

public sealed record GetProviderByIdQuery(long ProviderId) : IRequest<ProviderDto?>;
