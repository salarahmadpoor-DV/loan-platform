namespace Matchi.Application.Features.Providers.Queries;

public sealed record ProviderInboxLocationDto(
    string? Province,
    string? City,
    string? District);

public sealed record ProviderRequestInboxItemDto(
    long RequestId,
    string RequestType,
    string? ServiceSummary,
    string? CategorySummary,
    ProviderInboxLocationDto? Location,
    DateTime CreatedDate,
    string Status);

public sealed record ProviderProposalDto(
    long Id,
    long RequestId,
    string Status,
    decimal TotalPrice,
    DateTime CreateDate,
    long? DealId);

public sealed record ProviderDealDto(
    long Id,
    long RequestId,
    long ProposalId,
    string Status,
    decimal TotalPrice,
    DateTime AcceptedAt);

public sealed record ProviderExecutionDto(
    long Id,
    long DealId,
    long? BusinessId,
    string Status,
    DateOnly? ScheduledDate,
    TimeSpan? ScheduledTimeFrom,
    TimeSpan? ScheduledTimeTo,
    DateTime? StartedAt,
    DateTime? CompletedAt);
