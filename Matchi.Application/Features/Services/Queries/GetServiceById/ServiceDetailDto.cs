namespace Matchi.Application.Features.Services.Queries.GetServiceById;

public sealed class ServiceDetailDto
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public long CategoryId { get; init; }
    public int QuestionCount { get; init; }

    public ServiceDetailDto(long id, string name, long categoryId, int questionCount)
    {
        Id = id;
        Name = name;
        CategoryId = categoryId;
        QuestionCount = questionCount;
    }
}
