using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Deal : AuditableEntity
{
    public long RequestId { get; private set; };

    public long ProposalId { get; private set; };

    public long CustomerId { get; private set; };

    public string Status { get; private set; } = "Active";

    public decimal TotalPrice { get; private set; };

    public DateTime AcceptedAt { get; private set; };

    public DateTime? CompletedAt { get; private set; };

    public DateTime? CancelledAt { get; private set; };

    public Customer Customer { get; private set; } = null!;

    public Proposal Proposal { get; private set; } = null!;

    public Request Request { get; private set; } = null!;

    public ICollection<Cancellation> Cancellations { get; private set; } = new List<Cancellation>();

    public ICollection<Complaint> Complaints { get; private set; } = new List<Complaint>();

    public ICollection<ProductDelivery> ProductDeliveries { get; private set; } = new List<ProductDelivery>();

    public ICollection<Review> Reviews { get; private set; } = new List<Review>();

    public ICollection<ServiceExecution> ServiceExecutions { get; private set; } = new List<ServiceExecution>();

    private Deal()
    {
    }

    public Deal(long requestId, long proposalId, long customerId, decimal totalPrice)
    {
        RequestId = requestId;
        ProposalId = proposalId;
        CustomerId = customerId;
        TotalPrice = totalPrice;
    }
}
