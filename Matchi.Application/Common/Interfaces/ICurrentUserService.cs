namespace Matchi.Application.Common.Interfaces;

public interface ICurrentUserService
{
    bool IsAuthenticated { get; }

    long? UserId { get; }

    string? Mobile { get; }
}
