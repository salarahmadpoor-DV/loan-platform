using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class BusinessAvailability : AuditableEntity
{
    public long BusinessId { get; private set; }

    public byte DayOfWeek { get; private set; }

    public TimeSpan TimeFrom { get; private set; }

    public TimeSpan TimeTo { get; private set; }

    public bool IsAvailable { get; private set; } = true;

    public Business Business { get; private set; } = null!;

    private BusinessAvailability()
    {
    }

    public BusinessAvailability(long businessId, byte dayOfWeek, TimeSpan timeFrom, TimeSpan timeTo)
    {
        BusinessId = businessId;
        DayOfWeek = dayOfWeek;
        TimeFrom = timeFrom;
        TimeTo = timeTo;
        IsAvailable = true;
    }

    public void UpdateSlot(byte dayOfWeek, TimeSpan timeFrom, TimeSpan timeTo, bool isAvailable)
    {
        DayOfWeek = dayOfWeek;
        TimeFrom = timeFrom;
        TimeTo = timeTo;
        IsAvailable = isAvailable;
        SetUpdated();
    }
}
