using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ProductDelivery : TimestampedEntity
{
    public long DealId { get; private set; };

    public string Status { get; private set; } = "Pending";

    public string? Address { get; private set; };

    public string? Province { get; private set; };

    public string? City { get; private set; };

    public string? District { get; private set; };

    public decimal? Lat { get; private set; };

    public decimal? Lng { get; private set; };

    public DateOnly? ScheduledDate { get; private set; };

    public DateTime? DeliveredAt { get; private set; };

    public string? TrackingCode { get; private set; };

    public Deal Deal { get; private set; } = null!;

    private ProductDelivery()
    {
    }

    public ProductDelivery(long dealId)
    {
        DealId = dealId;
    }
}
