namespace Matchi.Application.Features.Deals;

public sealed record DealSummaryDto(
    long Id,
    long RequestId,
    long ProposalId,
    string Status,
    decimal TotalPrice,
    DateTime AcceptedAt);

public sealed record DealDetailDto(
    long Id,
    long RequestId,
    long ProposalId,
    long CustomerId,
    string Status,
    decimal TotalPrice,
    DateTime AcceptedAt,
    DateTime CreateDate,
    DateTime? UpdateDate);

internal static class DealDtoMapper
{
    public static DealSummaryDto ToSummary(Domain.Entities.Deal deal) =>
        new(
            deal.Id,
            deal.RequestId,
            deal.ProposalId,
            deal.Status,
            deal.TotalPrice,
            deal.AcceptedAt);

    public static DealDetailDto ToDetail(Domain.Entities.Deal deal) =>
        new(
            deal.Id,
            deal.RequestId,
            deal.ProposalId,
            deal.CustomerId,
            deal.Status,
            deal.TotalPrice,
            deal.AcceptedAt,
            deal.CreateDate,
            deal.UpdateDate);
}
