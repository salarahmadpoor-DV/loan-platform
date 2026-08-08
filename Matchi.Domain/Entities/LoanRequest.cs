using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class LoanRequest : AuditableEntity
{
    public long BankId { get; private set; }

    public long UserId { get; private set; }

    public string Status { get; private set; } = "Pending";


    private LoanRequest()
    {
    }

    public LoanRequest(
        long bankId,
        long userId)
    {
        BankId = bankId;
        UserId = userId;
    }
}