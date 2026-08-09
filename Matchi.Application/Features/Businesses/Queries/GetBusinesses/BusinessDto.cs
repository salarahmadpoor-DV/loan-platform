namespace Matchi.Application.Features.Businesses.Queries.GetBusinesses;

public sealed class BusinessDto
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Address { get; init; }

    public BusinessDto(long id, string name, string? address)
    {
        Id = id;
        Name = name;
        Address = address;
    }
}
