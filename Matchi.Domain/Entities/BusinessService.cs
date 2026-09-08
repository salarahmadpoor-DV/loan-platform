using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class BusinessService : AuditableEntity
{
    public long BusinessId { get; private set; }

    public long ServiceId { get; private set; }

    public bool IsActive { get; private set; } = true;

    public bool CanCustomerChooseProvider { get; private set; } = false;

    public decimal? MinPrice { get; private set; }

    public decimal? MaxPrice { get; private set; }

    public Business Business { get; private set; } = null!;

    public Service Service { get; private set; } = null!;

    private BusinessService()
    {
    }

    public BusinessService(long businessId, long serviceId)
    {
        BusinessId = businessId;
        ServiceId = serviceId;
        IsActive = true;
    }

    public void UpdateOffer(bool isActive, bool canCustomerChooseProvider, decimal? minPrice, decimal? maxPrice)
    {
        IsActive = isActive;
        CanCustomerChooseProvider = canCustomerChooseProvider;
        MinPrice = minPrice;
        MaxPrice = maxPrice;
        SetUpdated();
    }

    public void Reactivate(bool isActive, bool canCustomerChooseProvider, decimal? minPrice, decimal? maxPrice)
    {
        Restore();
        UpdateOffer(isActive, canCustomerChooseProvider, minPrice, maxPrice);
    }
}
