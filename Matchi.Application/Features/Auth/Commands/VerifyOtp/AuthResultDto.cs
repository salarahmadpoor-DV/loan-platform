namespace Matchi.Application.Features.Auth.Commands.VerifyOtp;

public sealed class AuthResultDto
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
    public DateTime ExpiresAtUtc { get; init; }
    public AuthUserDto User { get; init; } = null!;
}

public sealed class AuthUserDto
{
    public long Id { get; init; }
    public string Mobile { get; init; } = null!;
    public IEnumerable<string> Roles { get; init; } = Array.Empty<string>();
}
