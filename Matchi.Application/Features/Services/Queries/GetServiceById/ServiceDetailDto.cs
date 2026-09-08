namespace Matchi.Application.Features.Services.Queries.GetServiceById;

public sealed class ServiceDetailDto
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public long CategoryId { get; init; }
    public int AttributeCount { get; init; }

    public ServiceDetailDto(long id, string name, long categoryId, int attributeCount)
    {
        Id = id;
        Name = name;
        CategoryId = categoryId;
        AttributeCount = attributeCount;
    }
}
