using Matchi.Domain.Entities;

namespace Matchi.Application.Features.Requests;

public sealed record RequestServiceAttributeDto(long ServiceAttributeId, string? Value);

public sealed record RequestProductAttributeDto(long ProductAttributeId, string? Value);

public sealed record RequestServiceLineDto(
    long ServiceId,
    decimal Quantity = 1m,
    string? Description = null,
    int? DisplayOrder = null,
    IReadOnlyList<RequestServiceAttributeDto>? Attributes = null);

public sealed record RequestProductLineDto(
    long? ProductId,
    long? ProductCategoryId,
    decimal Quantity = 1m,
    string? Unit = null,
    string? Description = null,
    int? DisplayOrder = null,
    IReadOnlyList<RequestProductAttributeDto>? Attributes = null);

public sealed record RequestLocationDto(
    string? Province,
    string? City,
    string? District,
    string? Address,
    double? Lat,
    double? Lng);

public sealed record RequestScheduleDto(
    DateOnly Date,
    TimeSpan? TimeFrom,
    TimeSpan? TimeTo,
    bool IsFlexible);

public sealed record RequestServiceResultDto(
    long Id,
    long ServiceId,
    decimal Quantity,
    string? Description,
    int DisplayOrder,
    IReadOnlyList<RequestServiceAttributeDto> Attributes);

public sealed record RequestProductResultDto(
    long Id,
    long? ProductId,
    long? ProductCategoryId,
    decimal Quantity,
    string? Unit,
    string? Description,
    int DisplayOrder,
    IReadOnlyList<RequestProductAttributeDto> Attributes);

public sealed record RequestDto(
    long Id,
    long CustomerId,
    string RequestType,
    string Title,
    string? Description,
    string Status,
    IReadOnlyList<RequestServiceResultDto> Services,
    IReadOnlyList<RequestProductResultDto> Products,
    RequestLocationDto? Location,
    RequestScheduleDto? Schedule,
    DateTime CreateDate,
    DateTime? UpdateDate);

internal static class RequestDtoMapper
{
    public static RequestDto ToDto(Request request)
    {
        var location = request.Locations
            .OrderBy(x => x.Id)
            .FirstOrDefault();

        var schedule = request.Schedules
            .OrderBy(x => x.Id)
            .FirstOrDefault();

        return new RequestDto(
            request.Id,
            request.CustomerId,
            request.RequestType,
            request.Title,
            request.Description,
            request.Status,
            request.Services
                .OrderBy(s => s.DisplayOrder)
                .ThenBy(s => s.Id)
                .Select(s => new RequestServiceResultDto(
                    s.Id,
                    s.ServiceId,
                    s.Quantity,
                    s.Description,
                    s.DisplayOrder,
                    s.Attributes
                        .Select(a => new RequestServiceAttributeDto(a.ServiceAttributeId, a.Value))
                        .ToList()))
                .ToList(),
            request.Products
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.Id)
                .Select(p => new RequestProductResultDto(
                    p.Id,
                    p.ProductId,
                    p.ProductCategoryId,
                    p.Quantity,
                    p.Unit,
                    p.Description,
                    p.DisplayOrder,
                    p.Attributes
                        .Select(a => new RequestProductAttributeDto(a.ProductAttributeId, a.Value))
                        .ToList()))
                .ToList(),
            location is null
                ? null
                : new RequestLocationDto(
                    location.Province,
                    location.City,
                    location.District,
                    location.Address,
                    location.Lat is null ? null : (double)location.Lat.Value,
                    location.Lng is null ? null : (double)location.Lng.Value),
            schedule is null
                ? null
                : new RequestScheduleDto(
                    schedule.Date,
                    schedule.TimeFrom,
                    schedule.TimeTo,
                    schedule.IsFlexible),
            request.CreateDate,
            request.UpdateDate);
    }
}
