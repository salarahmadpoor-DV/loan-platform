using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class RequestSchedule : TimestampedEntity
{
    public long RequestId { get; private set; }

    public DateOnly Date { get; private set; }

    public TimeSpan? TimeFrom { get; private set; }

    public TimeSpan? TimeTo { get; private set; }

    public bool IsFlexible { get; private set; } = false;

    public Request Request { get; private set; } = null!;

    private RequestSchedule()
    {
    }

    public RequestSchedule(long requestId, DateOnly date)
    {
        RequestId = requestId;
        Date = date;
    }
}
