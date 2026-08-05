using Loan.Domain.Common;

namespace Loan.Domain.Entities;

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