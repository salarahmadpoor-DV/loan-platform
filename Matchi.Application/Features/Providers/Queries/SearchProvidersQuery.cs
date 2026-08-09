using MediatR;

namespace Matchi.Application.Features.Providers.Queries;

public sealed record SearchProvidersQuery(long? ServiceId, double? Lat, double? Lng, int RadiusKm, string? SortBy, int Page, int PageSize) : IRequest<IEnumerable<ProviderDto>>;
