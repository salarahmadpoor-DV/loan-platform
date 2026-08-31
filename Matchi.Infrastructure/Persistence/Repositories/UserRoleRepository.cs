
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using Matchi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

public sealed class UserRoleRepository : IUserRoleRepository
{
    private readonly MatchiDbContext _context;

    public UserRoleRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<string>> GetRoleCodesByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                x.Role.IsActive)
            .Select(x => x.Role.Code)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);
    }
            public async Task<IReadOnlyList<string>> GetPermissionCodesByUserIdAsync(
            long userId,
            CancellationToken cancellationToken = default)
        {
            return await _context.UserRoles
                .AsNoTracking()
                .Where(ur =>
                    ur.UserId == userId &&
                    ur.Role.IsActive)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Where(rp => rp.Permission.IsActive)
                .Select(rp => rp.Permission.Code)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync(cancellationToken);
        }
}

