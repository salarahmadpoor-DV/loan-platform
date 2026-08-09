using Matchi.Domain.Entities;

namespace Matchi.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByMobileAsync(
        string mobile,
        CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task<User> AddAsync(
        User user,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        User user,
        CancellationToken cancellationToken = default);
}
