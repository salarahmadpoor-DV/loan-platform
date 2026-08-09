namespace Matchi.Application.Features.Services.Queries.GetServiceCategories;

public sealed class ServiceCategoryDto
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;

    public ServiceCategoryDto(long id, string name)
    {
        Id = id;
        Name = name;
    }
}
