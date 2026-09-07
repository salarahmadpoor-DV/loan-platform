from pathlib import Path

ROOT = Path(r"E:\Armin\Matchi\Matchi-platform")
ENT = ROOT / "Matchi.Domain" / "Entities"
CFG = ROOT / "Matchi.Infrastructure" / "Persistence" / "Configurations"
ENT.mkdir(parents=True, exist_ok=True)
CFG.mkdir(parents=True, exist_ok=True)

def w(name, content, folder=ENT):
    (folder / name).write_text(content.replace("\r\n", "\n"), encoding="utf-8")

H = """using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;
"""

# Remaining entities
w("Media.cs", H + """
public class Media : Entity
{
    public string FileName { get; private set; } = null!;
    public string StorageKey { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long Size { get; private set; }
    public string? Url { get; private set; }
    public DateTime CreateDate { get; private set; }

    public ICollection<Business> LogoBusinesses { get; private set; } = new List<Business>();
    public ICollection<BusinessPortfolioMedia> BusinessPortfolioMedia { get; private set; } = new List<BusinessPortfolioMedia>();
    public ICollection<ProviderPortfolioMedia> ProviderPortfolioMedia { get; private set; } = new List<ProviderPortfolioMedia>();
    public ICollection<VerificationDocument> VerificationDocuments { get; private set; } = new List<VerificationDocument>();

    private Media() { }

    public Media(string fileName, string storageKey, string contentType, long size, string? url = null)
    {
        FileName = fileName;
        StorageKey = storageKey;
        ContentType = contentType;
        Size = size;
        Url = url;
        CreateDate = DateTime.UtcNow;
    }
}
""")

w("ServiceCategory.cs", H + """
public class ServiceCategory : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ICollection<Service> Services { get; private set; } = new List<Service>();

    private ServiceCategory() { }

    public ServiceCategory(string name, string slug, int displayOrder = 0)
    {
        Name = name;
        Slug = slug;
        DisplayOrder = displayOrder;
        IsActive = true;
    }
}
""")

w("Service.cs", H + """
public class Service : AuditableEntity
{
    public long CategoryId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Slug { get; private set; }
    public string? Description { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ServiceCategory Category { get; private set; } = null!;
    public ICollection<ServiceAttribute> Attributes { get; private set; } = new List<ServiceAttribute>();
    public ICollection<ProviderService> ProviderServices { get; private set; } = new List<ProviderService>();
    public ICollection<BusinessService> BusinessServices { get; private set; } = new List<BusinessService>();
    public ICollection<RequestService> RequestServices { get; private set; } = new List<RequestService>();
    public ICollection<ProposalItem> ProposalItems { get; private set; } = new List<ProposalItem>();

    private Service() { }

    public Service(string name, long categoryId, string? slug = null, string? description = null, int displayOrder = 0)
    {
        Name = name;
        CategoryId = categoryId;
        Slug = slug;
        Description = description;
        DisplayOrder = displayOrder;
        IsActive = true;
    }
}
""")

w("ServiceAttribute.cs", H + """
public class ServiceAttribute : AuditableEntity
{
    public long ServiceId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public string DataType { get; private set; } = null!;
    public bool IsRequired { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Service Service { get; private set; } = null!;
    public ICollection<ServiceAttributeOption> Options { get; private set; } = new List<ServiceAttributeOption>();
    public ICollection<ProviderCapability> ProviderCapabilities { get; private set; } = new List<ProviderCapability>();
    public ICollection<RequestServiceAttribute> RequestServiceAttributes { get; private set; } = new List<RequestServiceAttribute>();

    private ServiceAttribute() { }

    public ServiceAttribute(long serviceId, string name, string code, string dataType, bool isRequired = false, int displayOrder = 0)
    {
        ServiceId = serviceId;
        Name = name;
        Code = code;
        DataType = dataType;
        IsRequired = isRequired;
        DisplayOrder = displayOrder;
        IsActive = true;
    }
}
""")

w("ServiceAttributeOption.cs", H + """
public class ServiceAttributeOption : AuditableEntity
{
    public long ServiceAttributeId { get; private set; }
    public string Value { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ServiceAttribute ServiceAttribute { get; private set; } = null!;

    private ServiceAttributeOption() { }

    public ServiceAttributeOption(long serviceAttributeId, string value, string displayName, int displayOrder = 0)
    {
        ServiceAttributeId = serviceAttributeId;
        Value = value;
        DisplayName = displayName;
        DisplayOrder = displayOrder;
        IsActive = true;
    }
}
""")

w("ProductCategory.cs", H + """
public class ProductCategory : AuditableEntity
{
    public long? ParentId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public string? Description { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ProductCategory? Parent { get; private set; }
    public ICollection<ProductCategory> Children { get; private set; } = new List<ProductCategory>();
    public ICollection<Product> Products { get; private set; } = new List<Product>();
    public ICollection<ProductAttribute> Attributes { get; private set; } = new List<ProductAttribute>();
    public ICollection<RequestProduct> RequestProducts { get; private set; } = new List<RequestProduct>();

    private ProductCategory() { }

    public ProductCategory(string name, string slug, long? parentId = null, string? description = null, int displayOrder = 0)
    {
        Name = name;
        Slug = slug;
        ParentId = parentId;
        Description = description;
        DisplayOrder = displayOrder;
        IsActive = true;
    }
}
""")

w("Product.cs", H + """
public class Product : AuditableEntity
{
    public long CategoryId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Slug { get; private set; }
    public string? Description { get; private set; }
    public string? Brand { get; private set; }
    public string? Model { get; private set; }
    public string? Sku { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ProductCategory Category { get; private set; } = null!;
    public ICollection<ProductAttributeValue> AttributeValues { get; private set; } = new List<ProductAttributeValue>();
    public ICollection<ProviderProduct> ProviderProducts { get; private set; } = new List<ProviderProduct>();
    public ICollection<BusinessProduct> BusinessProducts { get; private set; } = new List<BusinessProduct>();
    public ICollection<RequestProduct> RequestProducts { get; private set; } = new List<RequestProduct>();
    public ICollection<ProposalItem> ProposalItems { get; private set; } = new List<ProposalItem>();

    private Product() { }

    public Product(long categoryId, string name, string? slug = null, string? sku = null)
    {
        CategoryId = categoryId;
        Name = name;
        Slug = slug;
        Sku = sku;
        IsActive = true;
    }
}
""")

w("ProductAttribute.cs", H + """
public class ProductAttribute : AuditableEntity
{
    public long ProductCategoryId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public string DataType { get; private set; } = null!;
    public bool IsRequired { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ProductCategory ProductCategory { get; private set; } = null!;
    public ICollection<ProductAttributeOption> Options { get; private set; } = new List<ProductAttributeOption>();
    public ICollection<ProductAttributeValue> Values { get; private set; } = new List<ProductAttributeValue>();
    public ICollection<RequestProductAttribute> RequestProductAttributes { get; private set; } = new List<RequestProductAttribute>();

    private ProductAttribute() { }

    public ProductAttribute(long productCategoryId, string name, string code, string dataType, bool isRequired = false, int displayOrder = 0)
    {
        ProductCategoryId = productCategoryId;
        Name = name;
        Code = code;
        DataType = dataType;
        IsRequired = isRequired;
        DisplayOrder = displayOrder;
        IsActive = true;
    }
}
""")

w("ProductAttributeOption.cs", H + """
public class ProductAttributeOption : AuditableEntity
{
    public long ProductAttributeId { get; private set; }
    public string Value { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ProductAttribute ProductAttribute { get; private set; } = null!;

    private ProductAttributeOption() { }

    public ProductAttributeOption(long productAttributeId, string value, string displayName, int displayOrder = 0)
    {
        ProductAttributeId = productAttributeId;
        Value = value;
        DisplayName = displayName;
        DisplayOrder = displayOrder;
        IsActive = true;
    }
}
""")

w("ProductAttributeValue.cs", H + """
public class ProductAttributeValue : AuditableEntity
{
    public long ProductId { get; private set; }
    public long ProductAttributeId { get; private set; }
    public string Value { get; private set; } = null!;

    public Product Product { get; private set; } = null!;
    public ProductAttribute ProductAttribute { get; private set; } = null!;

    private ProductAttributeValue() { }

    public ProductAttributeValue(long productId, long productAttributeId, string value)
    {
        ProductId = productId;
        ProductAttributeId = productAttributeId;
        Value = value;
    }
}
""")

w("ProviderService.cs", H + """
public class ProviderService : AuditableEntity
{
    public long ProviderId { get; private set; }
    public long ServiceId { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Provider Provider { get; private set; } = null!;
    public Service Service { get; private set; } = null!;

    private ProviderService() { }

    public ProviderService(long providerId, long serviceId)
    {
        ProviderId = providerId;
        ServiceId = serviceId;
        IsActive = true;
    }
}
""")

w("BusinessService.cs", H + """
public class BusinessService : AuditableEntity
{
    public long BusinessId { get; private set; }
    public long ServiceId { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool CanCustomerChooseProvider { get; private set; }
    public decimal? MinPrice { get; private set; }
    public decimal? MaxPrice { get; private set; }

    public Business Business { get; private set; } = null!;
    public Service Service { get; private set; } = null!;

    private BusinessService() { }

    public BusinessService(long businessId, long serviceId, bool canCustomerChooseProvider = false, decimal? minPrice = null, decimal? maxPrice = null)
    {
        BusinessId = businessId;
        ServiceId = serviceId;
        CanCustomerChooseProvider = canCustomerChooseProvider;
        MinPrice = minPrice;
        MaxPrice = maxPrice;
        IsActive = true;
    }
}
""")

w("ProviderCapability.cs", H + """
public class ProviderCapability : AuditableEntity
{
    public long ProviderId { get; private set; }
    public long ServiceAttributeId { get; private set; }
    public string Value { get; private set; } = null!;

    public Provider Provider { get; private set; } = null!;
    public ServiceAttribute ServiceAttribute { get; private set; } = null!;

    private ProviderCapability() { }

    public ProviderCapability(long providerId, long serviceAttributeId, string value)
    {
        ProviderId = providerId;
        ServiceAttributeId = serviceAttributeId;
        Value = value;
    }
}
""")

w("ProviderProduct.cs", H + """
public class ProviderProduct : AuditableEntity
{
    public long ProviderId { get; private set; }
    public long ProductId { get; private set; }
    public decimal? Price { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public decimal? MinOrderQuantity { get; private set; }
    public int? LeadTimeDays { get; private set; }

    public Provider Provider { get; private set; } = null!;
    public Product Product { get; private set; } = null!;

    private ProviderProduct() { }

    public ProviderProduct(long providerId, long productId, decimal? price = null)
    {
        ProviderId = providerId;
        ProductId = productId;
        Price = price;
        IsAvailable = true;
    }
}
""")

w("BusinessProduct.cs", H + """
public class BusinessProduct : AuditableEntity
{
    public long BusinessId { get; private set; }
    public long ProductId { get; private set; }
    public decimal? Price { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public decimal? MinOrderQuantity { get; private set; }
    public int? LeadTimeDays { get; private set; }

    public Business Business { get; private set; } = null!;
    public Product Product { get; private set; } = null!;

    private BusinessProduct() { }

    public BusinessProduct(long businessId, long productId, decimal? price = null)
    {
        BusinessId = businessId;
        ProductId = productId;
        Price = price;
        IsAvailable = true;
    }
}
""")

w("ProviderServiceArea.cs", H + """
public class ProviderServiceArea : AuditableEntity
{
    public long ProviderId { get; private set; }
    public string AreaType { get; private set; } = null!;
    public string? Province { get; private set; }
    public string? City { get; private set; }
    public string? District { get; private set; }
    public decimal? Lat { get; private set; }
    public decimal? Lng { get; private set; }
    public decimal? Radius { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Provider Provider { get; private set; } = null!;

    private ProviderServiceArea() { }

    public ProviderServiceArea(long providerId, string areaType)
    {
        ProviderId = providerId;
        AreaType = areaType;
        IsActive = true;
    }
}
""")

w("BusinessServiceArea.cs", H + """
public class BusinessServiceArea : AuditableEntity
{
    public long BusinessId { get; private set; }
    public string AreaType { get; private set; } = null!;
    public string? Province { get; private set; }
    public string? City { get; private set; }
    public string? District { get; private set; }
    public decimal? Lat { get; private set; }
    public decimal? Lng { get; private set; }
    public decimal? Radius { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Business Business { get; private set; } = null!;

    private BusinessServiceArea() { }

    public BusinessServiceArea(long businessId, string areaType)
    {
        BusinessId = businessId;
        AreaType = areaType;
        IsActive = true;
    }
}
""")

w("ProviderAvailability.cs", H + """
public class ProviderAvailability : AuditableEntity
{
    public long ProviderId { get; private set; }
    public byte DayOfWeek { get; private set; }
    public TimeSpan TimeFrom { get; private set; }
    public TimeSpan TimeTo { get; private set; }
    public bool IsAvailable { get; private set; } = true;

    public Provider Provider { get; private set; } = null!;

    private ProviderAvailability() { }

    public ProviderAvailability(long providerId, byte dayOfWeek, TimeSpan timeFrom, TimeSpan timeTo)
    {
        ProviderId = providerId;
        DayOfWeek = dayOfWeek;
        TimeFrom = timeFrom;
        TimeTo = timeTo;
        IsAvailable = true;
    }
}
""")

w("BusinessAvailability.cs", H + """
public class BusinessAvailability : AuditableEntity
{
    public long BusinessId { get; private set; }
    public byte DayOfWeek { get; private set; }
    public TimeSpan TimeFrom { get; private set; }
    public TimeSpan TimeTo { get; private set; }
    public bool IsAvailable { get; private set; } = true;

    public Business Business { get; private set; } = null!;

    private BusinessAvailability() { }

    public BusinessAvailability(long businessId, byte dayOfWeek, TimeSpan timeFrom, TimeSpan timeTo)
    {
        BusinessId = businessId;
        DayOfWeek = dayOfWeek;
        TimeFrom = timeFrom;
        TimeTo = timeTo;
        IsAvailable = true;
    }
}
""")

w("Request.cs", H + """
public class Request : AuditableEntity
{
    public long CustomerId { get; private set; }
    public string RequestType { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public string Status { get; private set; } = "Open";

    public Customer Customer { get; private set; } = null!;
    public ICollection<RequestService> RequestServices { get; private set; } = new List<RequestService>();
    public ICollection<RequestProduct> RequestProducts { get; private set; } = new List<RequestProduct>();
    public ICollection<RequestLocation> RequestLocations { get; private set; } = new List<RequestLocation>();
    public ICollection<RequestSchedule> RequestSchedules { get; private set; } = new List<RequestSchedule>();
    public ICollection<Proposal> Proposals { get; private set; } = new List<Proposal>();
    public ICollection<Deal> Deals { get; private set; } = new List<Deal>();
    public ICollection<Conversation> Conversations { get; private set; } = new List<Conversation>();
    public ICollection<Complaint> Complaints { get; private set; } = new List<Complaint>();
    public ICollection<Cancellation> Cancellations { get; private set; } = new List<Cancellation>();

    private Request() { }

    public Request(long customerId, string requestType, string title, string? description = null)
    {
        CustomerId = customerId;
        RequestType = requestType;
        Title = title;
        Description = description;
        Status = "Open";
    }
}
""")

w("RequestService.cs", H + """
public class RequestService : AuditableEntity
{
    public long RequestId { get; private set; }
    public long ServiceId { get; private set; }
    public decimal Quantity { get; private set; } = 1;
    public string? Description { get; private set; }
    public int DisplayOrder { get; private set; }

    public Request Request { get; private set; } = null!;
    public Service Service { get; private set; } = null!;
    public ICollection<RequestServiceAttribute> Attributes { get; private set; } = new List<RequestServiceAttribute>();

    private RequestService() { }

    public RequestService(long requestId, long serviceId, decimal quantity = 1, string? description = null, int displayOrder = 0)
    {
        RequestId = requestId;
        ServiceId = serviceId;
        Quantity = quantity;
        Description = description;
        DisplayOrder = displayOrder;
    }
}
""")

w("RequestProduct.cs", H + """
public class RequestProduct : AuditableEntity
{
    public long RequestId { get; private set; }
    public long? ProductId { get; private set; }
    public long? ProductCategoryId { get; private set; }
    public decimal Quantity { get; private set; } = 1;
    public string? Unit { get; private set; }
    public string? Description { get; private set; }
    public int DisplayOrder { get; private set; }

    public Request Request { get; private set; } = null!;
    public Product? Product { get; private set; }
    public ProductCategory? ProductCategory { get; private set; }
    public ICollection<RequestProductAttribute> Attributes { get; private set; } = new List<RequestProductAttribute>();

    private RequestProduct() { }

    public RequestProduct(long requestId, decimal quantity = 1, long? productId = null, long? productCategoryId = null)
    {
        RequestId = requestId;
        Quantity = quantity;
        ProductId = productId;
        ProductCategoryId = productCategoryId;
    }
}
""")

w("RequestServiceAttribute.cs", H + """
public class RequestServiceAttribute : AuditableEntity
{
    public long RequestServiceId { get; private set; }
    public long ServiceAttributeId { get; private set; }
    public string? Value { get; private set; }

    public RequestService RequestService { get; private set; } = null!;
    public ServiceAttribute ServiceAttribute { get; private set; } = null!;

    private RequestServiceAttribute() { }

    public RequestServiceAttribute(long requestServiceId, long serviceAttributeId, string? value)
    {
        RequestServiceId = requestServiceId;
        ServiceAttributeId = serviceAttributeId;
        Value = value;
    }
}
""")

w("RequestProductAttribute.cs", H + """
public class RequestProductAttribute : AuditableEntity
{
    public long RequestProductId { get; private set; }
    public long ProductAttributeId { get; private set; }
    public string? Value { get; private set; }

    public RequestProduct RequestProduct { get; private set; } = null!;
    public ProductAttribute ProductAttribute { get; private set; } = null!;

    private RequestProductAttribute() { }

    public RequestProductAttribute(long requestProductId, long productAttributeId, string? value)
    {
        RequestProductId = requestProductId;
        ProductAttributeId = productAttributeId;
        Value = value;
    }
}
""")

w("RequestLocation.cs", H + """
public class RequestLocation : TimestampedEntity
{
    public long RequestId { get; private set; }
    public string? Address { get; private set; }
    public string? Province { get; private set; }
    public string? City { get; private set; }
    public string? District { get; private set; }
    public decimal? Lat { get; private set; }
    public decimal? Lng { get; private set; }

    public Request Request { get; private set; } = null!;

    private RequestLocation() { }

    public RequestLocation(long requestId, decimal? lat = null, decimal? lng = null, string? address = null)
    {
        RequestId = requestId;
        Lat = lat;
        Lng = lng;
        Address = address;
    }
}
""")

w("RequestSchedule.cs", H + """
public class RequestSchedule : TimestampedEntity
{
    public long RequestId { get; private set; }
    public DateOnly Date { get; private set; }
    public TimeSpan? TimeFrom { get; private set; }
    public TimeSpan? TimeTo { get; private set; }
    public bool IsFlexible { get; private set; }

    public Request Request { get; private set; } = null!;

    private RequestSchedule() { }

    public RequestSchedule(long requestId, DateOnly date, TimeSpan? timeFrom = null, TimeSpan? timeTo = null, bool isFlexible = false)
    {
        RequestId = requestId;
        Date = date;
        TimeFrom = timeFrom;
        TimeTo = timeTo;
        IsFlexible = isFlexible;
    }
}
""")

w("Proposal.cs", H + """
public class Proposal : AuditableEntity
{
    public long RequestId { get; private set; }
    public long? BusinessId { get; private set; }
    public long? ProviderId { get; private set; }
    public decimal TotalPrice { get; private set; }
    public decimal DeliveryFee { get; private set; }
    public string? Message { get; private set; }
    public DateOnly? ProposedDate { get; private set; }
    public TimeSpan? ProposedTimeFrom { get; private set; }
    public TimeSpan? ProposedTimeTo { get; private set; }
    public string Status { get; private set; } = "Pending";
    public DateTime? ExpireAt { get; private set; }

    public Request Request { get; private set; } = null!;
    public Business? Business { get; private set; }
    public Provider? Provider { get; private set; }
    public ICollection<ProposalItem> Items { get; private set; } = new List<ProposalItem>();
    public Deal? Deal { get; private set; }

    private Proposal() { }

    public Proposal(long requestId, decimal totalPrice, long? businessId, long? providerId, decimal deliveryFee = 0)
    {
        RequestId = requestId;
        TotalPrice = totalPrice;
        BusinessId = businessId;
        ProviderId = providerId;
        DeliveryFee = deliveryFee;
        Status = "Pending";
    }
}
""")

w("ProposalItem.cs", H + """
public class ProposalItem : TimestampedEntity
{
    public long ProposalId { get; private set; }
    public string ItemType { get; private set; } = null!;
    public long? ProductId { get; private set; }
    public long? ServiceId { get; private set; }
    public string? Description { get; private set; }
    public decimal Quantity { get; private set; } = 1;
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }
    public int DisplayOrder { get; private set; }

    public Proposal Proposal { get; private set; } = null!;
    public Product? Product { get; private set; }
    public Service? Service { get; private set; }

    private ProposalItem() { }

    public ProposalItem(long proposalId, string itemType, decimal quantity, decimal unitPrice, decimal totalPrice, long? productId = null, long? serviceId = null)
    {
        ProposalId = proposalId;
        ItemType = itemType;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = totalPrice;
        ProductId = productId;
        ServiceId = serviceId;
    }
}
""")

w("Deal.cs", H + """
public class Deal : AuditableEntity
{
    public long RequestId { get; private set; }
    public long ProposalId { get; private set; }
    public long CustomerId { get; private set; }
    public string Status { get; private set; } = "Active";
    public decimal TotalPrice { get; private set; }
    public DateTime AcceptedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    public Request Request { get; private set; } = null!;
    public Proposal Proposal { get; private set; } = null!;
    public Customer Customer { get; private set; } = null!;
    public ICollection<ServiceExecution> ServiceExecutions { get; private set; } = new List<ServiceExecution>();
    public ICollection<ProductDelivery> ProductDeliveries { get; private set; } = new List<ProductDelivery>();
    public ICollection<Review> Reviews { get; private set; } = new List<Review>();
    public ICollection<Complaint> Complaints { get; private set; } = new List<Complaint>();
    public ICollection<Cancellation> Cancellations { get; private set; } = new List<Cancellation>();

    private Deal() { }

    public Deal(long requestId, long proposalId, long customerId, decimal totalPrice)
    {
        RequestId = requestId;
        ProposalId = proposalId;
        CustomerId = customerId;
        TotalPrice = totalPrice;
        Status = "Active";
        AcceptedAt = DateTime.UtcNow;
    }
}
""")

w("ServiceExecution.cs", H + """
public class ServiceExecution : TimestampedEntity
{
    public long DealId { get; private set; }
    public long? BusinessId { get; private set; }
    public string Status { get; private set; } = "Pending";
    public DateOnly? ScheduledDate { get; private set; }
    public TimeSpan? ScheduledTimeFrom { get; private set; }
    public TimeSpan? ScheduledTimeTo { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public Deal Deal { get; private set; } = null!;
    public Business? Business { get; private set; }
    public ICollection<ExecutionAssignment> Assignments { get; private set; } = new List<ExecutionAssignment>();

    private ServiceExecution() { }

    public ServiceExecution(long dealId, long? businessId = null)
    {
        DealId = dealId;
        BusinessId = businessId;
        Status = "Pending";
    }
}
""")

w("ExecutionAssignment.cs", H + """
public class ExecutionAssignment : TimestampedEntity
{
    public long ServiceExecutionId { get; private set; }
    public long ProviderId { get; private set; }
    public string Role { get; private set; } = null!;
    public bool IsPrimary { get; private set; }
    public string Status { get; private set; } = "Assigned";
    public DateTime AssignedAt { get; private set; }
    public DateTime? StartAt { get; private set; }
    public DateTime? EndAt { get; private set; }

    public ServiceExecution ServiceExecution { get; private set; } = null!;
    public Provider Provider { get; private set; } = null!;

    private ExecutionAssignment() { }

    public ExecutionAssignment(long serviceExecutionId, long providerId, string role, bool isPrimary = false)
    {
        ServiceExecutionId = serviceExecutionId;
        ProviderId = providerId;
        Role = role;
        IsPrimary = isPrimary;
        Status = "Assigned";
        AssignedAt = DateTime.UtcNow;
    }
}
""")

w("ProductDelivery.cs", H + """
public class ProductDelivery : TimestampedEntity
{
    public long DealId { get; private set; }
    public string Status { get; private set; } = "Pending";
    public string? Address { get; private set; }
    public string? Province { get; private set; }
    public string? City { get; private set; }
    public string? District { get; private set; }
    public decimal? Lat { get; private set; }
    public decimal? Lng { get; private set; }
    public DateOnly? ScheduledDate { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public string? TrackingCode { get; private set; }

    public Deal Deal { get; private set; } = null!;

    private ProductDelivery() { }

    public ProductDelivery(long dealId)
    {
        DealId = dealId;
        Status = "Pending";
    }
}
""")

w("Review.cs", H + """
public class Review : AuditableEntity
{
    public long DealId { get; private set; }
    public long CustomerId { get; private set; }
    public long? BusinessId { get; private set; }
    public long? ProviderId { get; private set; }
    public byte Rating { get; private set; }
    public string? Comment { get; private set; }

    public Deal Deal { get; private set; } = null!;
    public Customer Customer { get; private set; } = null!;
    public Business? Business { get; private set; }
    public Provider? Provider { get; private set; }

    private Review() { }

    public Review(long dealId, long customerId, byte rating, long? businessId, long? providerId, string? comment = null)
    {
        DealId = dealId;
        CustomerId = customerId;
        Rating = rating;
        BusinessId = businessId;
        ProviderId = providerId;
        Comment = comment;
    }
}
""")

w("Conversation.cs", H + """
public class Conversation : Entity
{
    public long RequestId { get; private set; }
    public long? BusinessId { get; private set; }
    public long CustomerId { get; private set; }
    public DateTime CreateDate { get; private set; }

    public Request Request { get; private set; } = null!;
    public Business? Business { get; private set; }
    public Customer Customer { get; private set; } = null!;
    public ICollection<ConversationParticipant> Participants { get; private set; } = new List<ConversationParticipant>();
    public ICollection<Message> Messages { get; private set; } = new List<Message>();

    private Conversation() { }

    public Conversation(long requestId, long customerId, long? businessId = null)
    {
        RequestId = requestId;
        CustomerId = customerId;
        BusinessId = businessId;
        CreateDate = DateTime.UtcNow;
    }
}
""")

w("ConversationParticipant.cs", H + """
public class ConversationParticipant : Entity
{
    public long ConversationId { get; private set; }
    public long UserId { get; private set; }
    public DateTime JoinedAt { get; private set; }
    public DateTime? LeftAt { get; private set; }

    public Conversation Conversation { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private ConversationParticipant() { }

    public ConversationParticipant(long conversationId, long userId)
    {
        ConversationId = conversationId;
        UserId = userId;
        JoinedAt = DateTime.UtcNow;
    }
}
""")

w("Message.cs", H + """
public class Message : Entity
{
    public long ConversationId { get; private set; }
    public long SenderUserId { get; private set; }
    public string Text { get; private set; } = null!;
    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Conversation Conversation { get; private set; } = null!;
    public User SenderUser { get; private set; } = null!;

    private Message() { }

    public Message(long conversationId, long senderUserId, string text)
    {
        ConversationId = conversationId;
        SenderUserId = senderUserId;
        Text = text;
        IsRead = false;
        CreatedAt = DateTime.UtcNow;
    }
}
""")

w("Complaint.cs", H + """
public class Complaint : TimestampedEntity
{
    public long RequestId { get; private set; }
    public long? DealId { get; private set; }
    public long CustomerId { get; private set; }
    public long? BusinessId { get; private set; }
    public long? ProviderId { get; private set; }
    public string Type { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Status { get; private set; } = "Open";
    public string? Resolution { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    public Request Request { get; private set; } = null!;
    public Deal? Deal { get; private set; }
    public Customer Customer { get; private set; } = null!;
    public Business? Business { get; private set; }
    public Provider? Provider { get; private set; }

    private Complaint() { }

    public Complaint(long requestId, long customerId, string type, string description, long? dealId = null, long? businessId = null, long? providerId = null)
    {
        RequestId = requestId;
        CustomerId = customerId;
        Type = type;
        Description = description;
        DealId = dealId;
        BusinessId = businessId;
        ProviderId = providerId;
        Status = "Open";
    }
}
""")

w("Cancellation.cs", H + """
public class Cancellation : Entity
{
    public long RequestId { get; private set; }
    public long? DealId { get; private set; }
    public long CancelledByUserId { get; private set; }
    public string Reason { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateTime CreateDate { get; private set; }

    public Request Request { get; private set; } = null!;
    public Deal? Deal { get; private set; }
    public User CancelledByUser { get; private set; } = null!;

    private Cancellation() { }

    public Cancellation(long requestId, long cancelledByUserId, string reason, long? dealId = null, string? description = null)
    {
        RequestId = requestId;
        CancelledByUserId = cancelledByUserId;
        Reason = reason;
        DealId = dealId;
        Description = description;
        CreateDate = DateTime.UtcNow;
    }
}
""")

w("BusinessPortfolio.cs", H + """
public class BusinessPortfolio : Entity
{
    public long BusinessId { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateTime CreateDate { get; private set; }

    public Business Business { get; private set; } = null!;
    public ICollection<BusinessPortfolioMedia> MediaItems { get; private set; } = new List<BusinessPortfolioMedia>();

    private BusinessPortfolio() { }

    public BusinessPortfolio(long businessId, string title, string? description = null)
    {
        BusinessId = businessId;
        Title = title;
        Description = description;
        CreateDate = DateTime.UtcNow;
    }
}
""")

w("BusinessPortfolioMedia.cs", H + """
public class BusinessPortfolioMedia : Entity
{
    public long PortfolioId { get; private set; }
    public long MediaId { get; private set; }
    public int DisplayOrder { get; private set; }

    public BusinessPortfolio Portfolio { get; private set; } = null!;
    public Media Media { get; private set; } = null!;

    private BusinessPortfolioMedia() { }

    public BusinessPortfolioMedia(long portfolioId, long mediaId, int displayOrder = 0)
    {
        PortfolioId = portfolioId;
        MediaId = mediaId;
        DisplayOrder = displayOrder;
    }
}
""")

w("ProviderPortfolio.cs", H + """
public class ProviderPortfolio : Entity
{
    public long ProviderId { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateTime CreateDate { get; private set; }

    public Provider Provider { get; private set; } = null!;
    public ICollection<ProviderPortfolioMedia> MediaItems { get; private set; } = new List<ProviderPortfolioMedia>();

    private ProviderPortfolio() { }

    public ProviderPortfolio(long providerId, string title, string? description = null)
    {
        ProviderId = providerId;
        Title = title;
        Description = description;
        CreateDate = DateTime.UtcNow;
    }
}
""")

w("ProviderPortfolioMedia.cs", H + """
public class ProviderPortfolioMedia : Entity
{
    public long PortfolioId { get; private set; }
    public long MediaId { get; private set; }
    public int DisplayOrder { get; private set; }

    public ProviderPortfolio Portfolio { get; private set; } = null!;
    public Media Media { get; private set; } = null!;

    private ProviderPortfolioMedia() { }

    public ProviderPortfolioMedia(long portfolioId, long mediaId, int displayOrder = 0)
    {
        PortfolioId = portfolioId;
        MediaId = mediaId;
        DisplayOrder = displayOrder;
    }
}
""")

w("Verification.cs", H + """
public class Verification : TimestampedEntity
{
    public string EntityType { get; private set; } = null!;
    public long EntityId { get; private set; }
    public string VerificationType { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public string? Provider { get; private set; }
    public string? Reference { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public DateTime? ExpireAt { get; private set; }
    public string? RejectReason { get; private set; }

    public ICollection<VerificationDocument> Documents { get; private set; } = new List<VerificationDocument>();

    private Verification() { }

    public Verification(string entityType, long entityId, string verificationType, string status)
    {
        EntityType = entityType;
        EntityId = entityId;
        VerificationType = verificationType;
        Status = status;
    }
}
""")

w("VerificationDocument.cs", H + """
public class VerificationDocument : Entity
{
    public long VerificationId { get; private set; }
    public long MediaId { get; private set; }
    public string DocumentType { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public DateTime UploadedAt { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public string? RejectReason { get; private set; }

    public Verification Verification { get; private set; } = null!;
    public Media Media { get; private set; } = null!;

    private VerificationDocument() { }

    public VerificationDocument(long verificationId, long mediaId, string documentType, string status)
    {
        VerificationId = verificationId;
        MediaId = mediaId;
        DocumentType = documentType;
        Status = status;
        UploadedAt = DateTime.UtcNow;
    }
}
""")

w("TrustScore.cs", H + """
public class TrustScore : Entity
{
    public string EntityType { get; private set; } = null!;
    public long EntityId { get; private set; }
    public decimal Score { get; private set; }
    public DateTime CalculatedAt { get; private set; }

    private TrustScore() { }

    public TrustScore(string entityType, long entityId, decimal score)
    {
        EntityType = entityType;
        EntityId = entityId;
        Score = score;
        CalculatedAt = DateTime.UtcNow;
    }
}
""")

print("entities written", len(list(ENT.glob('*.cs'))))
