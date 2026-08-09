using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class RequestAnswer : Entity
{
    public long ServiceRequestId { get; private set; }
    public ServiceRequest ServiceRequest { get; private set; } = null!;

    public long ServiceQuestionId { get; private set; }
    public long? SelectedOptionId { get; private set; }
    public QuestionOption? SelectedOption { get; private set; }

    public string? Text { get; private set; }

    private RequestAnswer() { }

    public RequestAnswer(long serviceRequestId, long questionId, long? optionId, string? text)
    {
        ServiceRequestId = serviceRequestId;
        ServiceQuestionId = questionId;
        SelectedOptionId = optionId;
        Text = text;
    }
}