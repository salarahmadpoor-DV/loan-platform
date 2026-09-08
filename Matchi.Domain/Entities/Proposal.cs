using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Proposal : AuditableEntity
{
    public long RequestId { get; private set; }

    public long? BusinessId { get; private set; }

    public long? ProviderId { get; private set; }

    public decimal TotalPrice { get; private set; }

    public decimal DeliveryFee { get; private set; } = 0m;

    public string? Message { get; private set; }

    public DateOnly? ProposedDate { get; private set; }

    public TimeSpan? ProposedTimeFrom { get; private set; }

    public TimeSpan? ProposedTimeTo { get; private set; }

    public string Status { get; private set; } = "Pending";

    public DateTime? ExpireAt { get; private set; }

    public Deal? Deal { get; private set; }

    public Business? Business { get; private set; }

    public Provider? Provider { get; private set; }

    public Request Request { get; private set; } = null!;

    public ICollection<ProposalItem> Items { get; private set; } = new List<ProposalItem>();

    private Proposal()
    {
    }

    private Proposal(
        long requestId,
        long? providerId,
        long? businessId,
        decimal totalPrice,
        decimal deliveryFee,
        string? message,
        DateOnly? proposedDate,
        TimeSpan? proposedTimeFrom,
        TimeSpan? proposedTimeTo,
        DateTime? expireAt)
    {
        if ((providerId is null) == (businessId is null))
            throw new InvalidOperationException("A proposal must have exactly one of ProviderId or BusinessId.");

        RequestId = requestId;
        ProviderId = providerId;
        BusinessId = businessId;
        TotalPrice = totalPrice;
        DeliveryFee = deliveryFee;
        Message = message;
        ProposedDate = proposedDate;
        ProposedTimeFrom = proposedTimeFrom;
        ProposedTimeTo = proposedTimeTo;
        ExpireAt = expireAt;
        Status = "Pending";
    }

    public static Proposal ForProvider(
        long requestId,
        long providerId,
        decimal totalPrice,
        decimal deliveryFee = 0m,
        string? message = null,
        DateOnly? proposedDate = null,
        TimeSpan? proposedTimeFrom = null,
        TimeSpan? proposedTimeTo = null,
        DateTime? expireAt = null)
    {
        return new Proposal(
            requestId,
            providerId,
            null,
            totalPrice,
            deliveryFee,
            message,
            proposedDate,
            proposedTimeFrom,
            proposedTimeTo,
            expireAt);
    }

    public static Proposal ForBusiness(
        long requestId,
        long businessId,
        decimal totalPrice,
        decimal deliveryFee = 0m,
        string? message = null,
        DateOnly? proposedDate = null,
        TimeSpan? proposedTimeFrom = null,
        TimeSpan? proposedTimeTo = null,
        DateTime? expireAt = null)
    {
        return new Proposal(
            requestId,
            null,
            businessId,
            totalPrice,
            deliveryFee,
            message,
            proposedDate,
            proposedTimeFrom,
            proposedTimeTo,
            expireAt);
    }

    public void AddItem(ProposalItem item) => Items.Add(item);

    public void Accept()
    {
        if (!string.Equals(Status, "Pending", StringComparison.Ordinal))
            throw new InvalidOperationException("Only a pending proposal can be accepted.");

        Status = "Accepted";
        SetUpdated();
    }

    public void Reject()
    {
        if (!string.Equals(Status, "Pending", StringComparison.Ordinal))
            throw new InvalidOperationException("Only a pending proposal can be rejected.");

        Status = "Rejected";
        SetUpdated();
    }
}
