namespace Matchi.Application.Features.Requests.Queries.GetServiceRequestsByUser;

public sealed class ServiceRequestSummaryDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string Status { get; init; } = null!;
    public long ServiceId { get; init; }
    public DateTime CreateDate { get; init; }

    public ServiceRequestSummaryDto(long id, string title, string status, long serviceId, DateTime createDate)
    {
        Id = id;
        Title = title;
        Status = status;
        ServiceId = serviceId;
        CreateDate = createDate;
    }
}
