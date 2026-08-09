namespace Matchi.Application.Features.Users.Queries;

public sealed class UserDto
{
    public long Id { get; init; }
    public string Mobile { get; init; } = null!;
    public string? Name { get; init; }

    public UserDto(long id, string mobile, string? name)
    {
        Id = id;
        Mobile = mobile;
        Name = name;
    }
}
