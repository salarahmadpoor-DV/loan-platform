namespace Matchi.Application.Features.Proposals;

public sealed record CreateProposalItemDto(
    string ItemType,
    long? ProductId = null,
    long? ServiceId = null,
    string? Description = null,
    decimal Quantity = 1m,
    decimal UnitPrice = 0m,
    decimal TotalPrice = 0m,
    int DisplayOrder = 0);

public sealed record ProposalItemDto(
    long Id,
    string ItemType,
    long? ProductId,
    long? ServiceId,
    string? Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    int DisplayOrder);

public sealed record ProposalListDto(
    long Id,
    long RequestId,
    string ProposerType,
    long ProposerId,
    decimal TotalPrice,
    decimal DeliveryFee,
    string Status,
    DateTime? ExpireAt,
    DateTime CreateDate);

public sealed record ProposalDetailDto(
    long Id,
    long RequestId,
    string ProposerType,
    long ProposerId,
    decimal TotalPrice,
    decimal DeliveryFee,
    string? Message,
    DateOnly? ProposedDate,
    TimeSpan? ProposedTimeFrom,
    TimeSpan? ProposedTimeTo,
    string Status,
    DateTime? ExpireAt,
    DateTime CreateDate,
    DateTime? UpdateDate,
    IReadOnlyList<ProposalItemDto> Items);

internal static class ProposalDtoMapper
{
    public static string ProposerType(Domain.Entities.Proposal proposal) =>
        proposal.ProviderId.HasValue ? "Provider" : "Business";

    public static long ProposerId(Domain.Entities.Proposal proposal) =>
        proposal.ProviderId ?? proposal.BusinessId
        ?? throw new InvalidOperationException("Proposal is missing a proposer.");

    public static ProposalListDto ToListDto(Domain.Entities.Proposal proposal) =>
        new(
            proposal.Id,
            proposal.RequestId,
            ProposerType(proposal),
            ProposerId(proposal),
            proposal.TotalPrice,
            proposal.DeliveryFee,
            proposal.Status,
            proposal.ExpireAt,
            proposal.CreateDate);

    public static ProposalDetailDto ToDetailDto(Domain.Entities.Proposal proposal) =>
        new(
            proposal.Id,
            proposal.RequestId,
            ProposerType(proposal),
            ProposerId(proposal),
            proposal.TotalPrice,
            proposal.DeliveryFee,
            proposal.Message,
            proposal.ProposedDate,
            proposal.ProposedTimeFrom,
            proposal.ProposedTimeTo,
            proposal.Status,
            proposal.ExpireAt,
            proposal.CreateDate,
            proposal.UpdateDate,
            proposal.Items
                .OrderBy(i => i.DisplayOrder)
                .ThenBy(i => i.Id)
                .Select(i => new ProposalItemDto(
                    i.Id,
                    i.ItemType,
                    i.ProductId,
                    i.ServiceId,
                    i.Description,
                    i.Quantity,
                    i.UnitPrice,
                    i.TotalPrice,
                    i.DisplayOrder))
                .ToList());
}
