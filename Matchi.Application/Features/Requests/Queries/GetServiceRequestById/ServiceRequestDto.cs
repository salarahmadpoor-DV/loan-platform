namespace Matchi.Application.Features.Requests.Queries.GetServiceRequestById;

public sealed class ServiceRequestDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
    public long ServiceId { get; init; }
    public long UserId { get; init; }
    public double? Lat { get; init; }
    public double? Lng { get; init; }
    public string Status { get; init; } = null!;
    public IEnumerable<ServiceRequestAnswerDto> Answers { get; init; } = Array.Empty<ServiceRequestAnswerDto>();

    public ServiceRequestDto(
        long id,
        string title,
        string? description,
        long serviceId,
        long userId,
        double? lat,
        double? lng,
        string status,
        IEnumerable<ServiceRequestAnswerDto> answers)
    {
        Id = id;
        Title = title;
        Description = description;
        ServiceId = serviceId;
        UserId = userId;
        Lat = lat;
        Lng = lng;
        Status = status;
        Answers = answers;
    }
}

public sealed class ServiceRequestAnswerDto
{
    public long QuestionId { get; init; }
    public long? OptionId { get; init; }
    public string? Text { get; init; }

    public ServiceRequestAnswerDto(long questionId, long? optionId, string? text)
    {
        QuestionId = questionId;
        OptionId = optionId;
        Text = text;
    }
}
