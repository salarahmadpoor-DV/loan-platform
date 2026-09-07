using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ProviderAvailability : AuditableEntity
{
    public long ProviderId { get; private set; };

    public byte DayOfWeek { get; private set; };

    public TimeSpan TimeFrom { get; private set; };

    public TimeSpan TimeTo { get; private set; };

    public bool IsAvailable { get; private set; } = true;

    public Provider Provider { get; private set; } = null!;

    private ProviderAvailability()
    {
    }

    public ProviderAvailability(long providerId, byte dayOfWeek, TimeSpan timeFrom, TimeSpan timeTo)
    {
        ProviderId = providerId;
        DayOfWeek = dayOfWeek;
        TimeFrom = timeFrom;
        TimeTo = timeTo;
    }
}
