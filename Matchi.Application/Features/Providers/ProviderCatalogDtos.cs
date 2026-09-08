namespace Matchi.Application.Features.Providers;

public sealed record ProviderProfileDto(
    long Id,
    long UserId,
    string Name,
    string Mobile,
    string? Description,
    double? Lat,
    double? Lng,
    decimal Rating,
    int ReviewCount,
    int CompletedJobCount,
    string Status);

public sealed record ProviderServiceDto(long ServiceId, string? ServiceName, bool IsActive);

public sealed record ProviderProductDto(
    long ProductId,
    string? ProductName,
    decimal? Price,
    bool IsAvailable,
    decimal? MinOrderQuantity,
    int? LeadTimeDays);

public sealed record ProviderCapabilityDto(
    long ServiceAttributeId,
    long ServiceId,
    string? AttributeName,
    string Value);

public sealed record ProviderServiceAreaDto(
    long Id,
    string AreaType,
    string? Province,
    string? City,
    string? District,
    double? Lat,
    double? Lng,
    decimal? Radius,
    bool IsActive);

public sealed record ProviderAvailabilityDto(
    long Id,
    byte DayOfWeek,
    TimeSpan TimeFrom,
    TimeSpan TimeTo,
    bool IsAvailable);

public sealed record ProviderMembershipDto(
    long BusinessId,
    string? BusinessName,
    string Role,
    string Status,
    DateTime JoinedAt,
    DateTime? LeftAt);
