using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class BusinessService : AuditableEntity
{
    public long BusinessId { get; private set; };

    public long ServiceId { get; private set; };

    public bool IsActive { get; private set; } = true;

    public bool CanCustomerChooseProvider { get; private set; } = 0;

    public decimal? MinPrice { get; private set; };

    public decimal? MaxPrice { get; private set; };

    public Business Business { get; private set; } = null!;

    public Service Service { get; private set; } = null!;

    private BusinessService()
    {
    }

    public BusinessService(long businessId, long serviceId)
    {
        BusinessId = businessId;
        ServiceId = serviceId;
    }
}
