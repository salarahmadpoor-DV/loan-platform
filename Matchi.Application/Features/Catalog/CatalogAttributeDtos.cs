namespace Matchi.Application.Features.Catalog;

public sealed record CatalogAttributeOptionDto(
    long Id,
    string Value,
    string DisplayName,
    int DisplayOrder);

public sealed record CatalogAttributeDto(
    long Id,
    string Name,
    string Code,
    string DataType,
    bool IsRequired,
    int DisplayOrder,
    IReadOnlyList<CatalogAttributeOptionDto> Options);
