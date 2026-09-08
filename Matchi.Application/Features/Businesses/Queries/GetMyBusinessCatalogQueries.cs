using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Businesses;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Businesses.Queries;

public sealed record GetMyBusinessServicesQuery(long? BusinessId) : IRequest<IReadOnlyList<BusinessServiceDto>>;

public sealed class GetMyBusinessServicesQueryHandler
    : IRequestHandler<GetMyBusinessServicesQuery, IReadOnlyList<BusinessServiceDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public GetMyBusinessServicesQueryHandler(ICurrentUserService currentUserService, IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<IReadOnlyList<BusinessServiceDto>> Handle(
        GetMyBusinessServicesQuery query,
        CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, query.BusinessId, cancellationToken);
        var items = await _businesses.ListServicesAsync(business.Id, cancellationToken);
        return items
            .Select(x => new BusinessServiceDto(
                x.ServiceId, x.Service.Name, x.IsActive, x.CanCustomerChooseProvider, x.MinPrice, x.MaxPrice))
            .ToList();
    }
}

public sealed record GetMyBusinessProductsQuery(long? BusinessId) : IRequest<IReadOnlyList<BusinessProductDto>>;

public sealed class GetMyBusinessProductsQueryHandler
    : IRequestHandler<GetMyBusinessProductsQuery, IReadOnlyList<BusinessProductDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public GetMyBusinessProductsQueryHandler(ICurrentUserService currentUserService, IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<IReadOnlyList<BusinessProductDto>> Handle(
        GetMyBusinessProductsQuery query,
        CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, query.BusinessId, cancellationToken);
        var items = await _businesses.ListProductsAsync(business.Id, cancellationToken);
        return items
            .Select(x => new BusinessProductDto(
                x.ProductId, x.Product.Name, x.Price, x.IsAvailable, x.MinOrderQuantity, x.LeadTimeDays))
            .ToList();
    }
}

public sealed record GetMyBusinessServiceAreasQuery(long? BusinessId) : IRequest<IReadOnlyList<BusinessServiceAreaDto>>;

public sealed class GetMyBusinessServiceAreasQueryHandler
    : IRequestHandler<GetMyBusinessServiceAreasQuery, IReadOnlyList<BusinessServiceAreaDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public GetMyBusinessServiceAreasQueryHandler(ICurrentUserService currentUserService, IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<IReadOnlyList<BusinessServiceAreaDto>> Handle(
        GetMyBusinessServiceAreasQuery query,
        CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, query.BusinessId, cancellationToken);
        var items = await _businesses.ListServiceAreasAsync(business.Id, cancellationToken);
        return items
            .Select(x => new BusinessServiceAreaDto(
                x.Id,
                x.AreaType,
                x.Province,
                x.City,
                x.District,
                x.Lat is null ? null : (double)x.Lat.Value,
                x.Lng is null ? null : (double)x.Lng.Value,
                x.Radius,
                x.IsActive))
            .ToList();
    }
}

public sealed record GetMyBusinessAvailabilitiesQuery(long? BusinessId)
    : IRequest<IReadOnlyList<BusinessAvailabilityDto>>;

public sealed class GetMyBusinessAvailabilitiesQueryHandler
    : IRequestHandler<GetMyBusinessAvailabilitiesQuery, IReadOnlyList<BusinessAvailabilityDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public GetMyBusinessAvailabilitiesQueryHandler(ICurrentUserService currentUserService, IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<IReadOnlyList<BusinessAvailabilityDto>> Handle(
        GetMyBusinessAvailabilitiesQuery query,
        CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, query.BusinessId, cancellationToken);
        var items = await _businesses.ListAvailabilitiesAsync(business.Id, cancellationToken);
        return items
            .Select(x => new BusinessAvailabilityDto(x.Id, x.DayOfWeek, x.TimeFrom, x.TimeTo, x.IsAvailable))
            .ToList();
    }
}

public sealed record GetMyBusinessProvidersQuery(long? BusinessId) : IRequest<IReadOnlyList<BusinessMembershipDto>>;

public sealed class GetMyBusinessProvidersQueryHandler
    : IRequestHandler<GetMyBusinessProvidersQuery, IReadOnlyList<BusinessMembershipDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public GetMyBusinessProvidersQueryHandler(ICurrentUserService currentUserService, IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<IReadOnlyList<BusinessMembershipDto>> Handle(
        GetMyBusinessProvidersQuery query,
        CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, query.BusinessId, cancellationToken);
        var items = await _businesses.ListMembershipsAsync(business.Id, cancellationToken);
        return items
            .Select(x => new BusinessMembershipDto(
                x.ProviderId, x.Provider.Name, x.Role, x.Status, x.JoinedAt, x.LeftAt))
            .ToList();
    }
}
