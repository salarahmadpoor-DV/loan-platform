using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Providers.Queries;

public sealed record GetMyProviderServicesQuery : IRequest<IReadOnlyList<ProviderServiceDto>>;

public sealed class GetMyProviderServicesQueryHandler
    : IRequestHandler<GetMyProviderServicesQuery, IReadOnlyList<ProviderServiceDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public GetMyProviderServicesQueryHandler(
        ICurrentUserService currentUserService,
        IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<IReadOnlyList<ProviderServiceDto>> Handle(
        GetMyProviderServicesQuery query,
        CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        var items = await _providers.ListServicesAsync(provider.Id, cancellationToken);
        return items
            .Select(x => new ProviderServiceDto(x.ServiceId, x.Service.Name, x.IsActive))
            .ToList();
    }
}

public sealed record GetMyProviderProductsQuery : IRequest<IReadOnlyList<ProviderProductDto>>;

public sealed class GetMyProviderProductsQueryHandler
    : IRequestHandler<GetMyProviderProductsQuery, IReadOnlyList<ProviderProductDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public GetMyProviderProductsQueryHandler(
        ICurrentUserService currentUserService,
        IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<IReadOnlyList<ProviderProductDto>> Handle(
        GetMyProviderProductsQuery query,
        CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        var items = await _providers.ListProductsAsync(provider.Id, cancellationToken);
        return items
            .Select(x => new ProviderProductDto(
                x.ProductId,
                x.Product.Name,
                x.Price,
                x.IsAvailable,
                x.MinOrderQuantity,
                x.LeadTimeDays))
            .ToList();
    }
}

public sealed record GetMyProviderCapabilitiesQuery : IRequest<IReadOnlyList<ProviderCapabilityDto>>;

public sealed class GetMyProviderCapabilitiesQueryHandler
    : IRequestHandler<GetMyProviderCapabilitiesQuery, IReadOnlyList<ProviderCapabilityDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public GetMyProviderCapabilitiesQueryHandler(
        ICurrentUserService currentUserService,
        IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<IReadOnlyList<ProviderCapabilityDto>> Handle(
        GetMyProviderCapabilitiesQuery query,
        CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        var items = await _providers.ListCapabilitiesAsync(provider.Id, cancellationToken);
        return items
            .Select(x => new ProviderCapabilityDto(
                x.ServiceAttributeId,
                x.ServiceAttribute.ServiceId,
                x.ServiceAttribute.Name,
                x.Value))
            .ToList();
    }
}

public sealed record GetMyProviderServiceAreasQuery : IRequest<IReadOnlyList<ProviderServiceAreaDto>>;

public sealed class GetMyProviderServiceAreasQueryHandler
    : IRequestHandler<GetMyProviderServiceAreasQuery, IReadOnlyList<ProviderServiceAreaDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public GetMyProviderServiceAreasQueryHandler(
        ICurrentUserService currentUserService,
        IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<IReadOnlyList<ProviderServiceAreaDto>> Handle(
        GetMyProviderServiceAreasQuery query,
        CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        var items = await _providers.ListServiceAreasAsync(provider.Id, cancellationToken);
        return items.Select(ToAreaDto).ToList();
    }

    internal static ProviderServiceAreaDto ToAreaDto(Domain.Entities.ProviderServiceArea x) =>
        new(
            x.Id,
            x.AreaType,
            x.Province,
            x.City,
            x.District,
            x.Lat is null ? null : (double)x.Lat.Value,
            x.Lng is null ? null : (double)x.Lng.Value,
            x.Radius,
            x.IsActive);
}

public sealed record GetMyProviderAvailabilitiesQuery : IRequest<IReadOnlyList<ProviderAvailabilityDto>>;

public sealed class GetMyProviderAvailabilitiesQueryHandler
    : IRequestHandler<GetMyProviderAvailabilitiesQuery, IReadOnlyList<ProviderAvailabilityDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public GetMyProviderAvailabilitiesQueryHandler(
        ICurrentUserService currentUserService,
        IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<IReadOnlyList<ProviderAvailabilityDto>> Handle(
        GetMyProviderAvailabilitiesQuery query,
        CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        var items = await _providers.ListAvailabilitiesAsync(provider.Id, cancellationToken);
        return items
            .Select(x => new ProviderAvailabilityDto(x.Id, x.DayOfWeek, x.TimeFrom, x.TimeTo, x.IsAvailable))
            .ToList();
    }
}
