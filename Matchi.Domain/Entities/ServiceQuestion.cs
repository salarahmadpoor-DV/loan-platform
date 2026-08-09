using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ServiceQuestion : AuditableEntity
{
    public long ServiceId { get; private set; }
    public Service Service { get; private set; } = null!;

    public string Text { get; private set; } = null!;

    public ICollection<QuestionOption> Options { get; private set; } = new List<QuestionOption>();

    private ServiceQuestion() { }

    public ServiceQuestion(long serviceId, string text)
    {
        ServiceId = serviceId;
        Text = text;
    }
}