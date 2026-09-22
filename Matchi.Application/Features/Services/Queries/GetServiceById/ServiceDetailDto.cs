using Matchi.Application.Features.Catalog;

namespace Matchi.Application.Features.Services.Queries.GetServiceById;

public sealed class ServiceDetailDto
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public long CategoryId { get; init; }
    public int AttributeCount { get; init; }
    public IReadOnlyList<CatalogAttributeDto> Attributes { get; init; }

    public ServiceDetailDto(
        long id,
        string name,
        long categoryId,
        int attributeCount,
        IReadOnlyList<CatalogAttributeDto>? attributes = null)
    {
        Id = id;
        Name = name;
        CategoryId = categoryId;
        AttributeCount = attributeCount;
        Attributes = attributes ?? Array.Empty<CatalogAttributeDto>();
    }
}
