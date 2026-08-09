using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class QuestionOption : AuditableEntity
{
    public long ServiceQuestionId { get; private set; }
    public ServiceQuestion ServiceQuestion { get; private set; } = null!;

    public string Text { get; private set; } = null!;

    private QuestionOption() { }

    public QuestionOption(long questionId, string text)
    {
        ServiceQuestionId = questionId;
        Text = text;
    }
}