using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class BankQuestion : AuditableEntity
{
    public long BankId { get; private set; }

    public string QuestionText { get; private set; } = null!;

    public bool IsRequired { get; private set; }

    public int SortOrder { get; private set; }


    private BankQuestion()
    {
    }

    public BankQuestion(
        long bankId,
        string questionText,
        bool isRequired,
        int sortOrder)
    {
        BankId = bankId;
        QuestionText = questionText;
        IsRequired = isRequired;
        SortOrder = sortOrder;
    }
}