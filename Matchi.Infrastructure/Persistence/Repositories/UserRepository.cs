using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using Matchi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MatchiDbContext _context;

    public UserRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByMobileAsync(
        string mobile,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(
                u => u.Mobile == mobile && !u.IsDeleted,
                cancellationToken);
    }

    public async Task<User> AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<User?> GetByIdAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
    }

    public async Task UpdateAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
