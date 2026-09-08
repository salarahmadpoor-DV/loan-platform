using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Businesses;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Businesses.Queries;

public sealed record GetMyBusinessesQuery : IRequest<IReadOnlyList<BusinessProfileDto>>;

public sealed class GetMyBusinessesQueryHandler : IRequestHandler<GetMyBusinessesQuery, IReadOnlyList<BusinessProfileDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public GetMyBusinessesQueryHandler(
        ICurrentUserService currentUserService,
        IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<IReadOnlyList<BusinessProfileDto>> Handle(
        GetMyBusinessesQuery query,
        CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        var owned = await _businesses.GetByOwnerUserIdAsync(userId, cancellationToken);
        return owned.Select(Map).ToList();
    }

    internal static BusinessProfileDto Map(Domain.Entities.Business business) =>
        new(
            business.Id,
            business.OwnerUserId,
            business.Name,
            business.Description,
            business.Mobile,
            business.Address,
            business.Province,
            business.City,
            business.District,
            business.Lat is null ? null : (double)business.Lat.Value,
            business.Lng is null ? null : (double)business.Lng.Value,
            business.LogoMediaId,
            business.Rating,
            business.ReviewCount,
            business.CompletedJobCount,
            business.Status);
}
