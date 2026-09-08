using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Businesses;

internal static class BusinessAccess
{
    public static async Task<Business> RequireOwned(
        ICurrentUserService currentUser,
        IBusinessRepository businesses,
        long? businessId,
        CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(currentUser);
        var owned = await businesses.GetByOwnerUserIdAsync(userId, cancellationToken);
        if (owned.Count == 0)
            throw new KeyNotFoundException("Business was not found.");

        if (businessId is > 0)
        {
            return owned.FirstOrDefault(b => b.Id == businessId.Value)
                ?? throw new KeyNotFoundException("Business was not found.");
        }

        if (owned.Count > 1)
            throw Fail.Validation("businessId", "Multiple businesses exist; pass businessId.");

        return owned[0];
    }
}

public sealed record BusinessProfileDto(
    long Id,
    long OwnerUserId,
    string Name,
    string? Description,
    string? Mobile,
    string? Address,
    string? Province,
    string? City,
    string? District,
    double? Lat,
    double? Lng,
    long? LogoMediaId,
    decimal Rating,
    int ReviewCount,
    int CompletedJobCount,
    string Status);

public sealed record BusinessServiceDto(
    long ServiceId,
    string? ServiceName,
    bool IsActive,
    bool CanCustomerChooseProvider,
    decimal? MinPrice,
    decimal? MaxPrice);

public sealed record BusinessProductDto(
    long ProductId,
    string? ProductName,
    decimal? Price,
    bool IsAvailable,
    decimal? MinOrderQuantity,
    int? LeadTimeDays);

public sealed record BusinessServiceAreaDto(
    long Id,
    string AreaType,
    string? Province,
    string? City,
    string? District,
    double? Lat,
    double? Lng,
    decimal? Radius,
    bool IsActive);

public sealed record BusinessAvailabilityDto(
    long Id,
    byte DayOfWeek,
    TimeSpan TimeFrom,
    TimeSpan TimeTo,
    bool IsAvailable);

public sealed record BusinessMembershipDto(
    long ProviderId,
    string? ProviderName,
    string Role,
    string Status,
    DateTime JoinedAt,
    DateTime? LeftAt);
