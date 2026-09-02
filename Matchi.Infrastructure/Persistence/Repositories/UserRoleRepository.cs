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
            .Where(x =>
                x.UserId == userId &&
                x.Role.IsActive)
            .SelectMany(x => x.Role.RolePermissions)
            .Where(x => x.Permission.IsActive)
            .Select(x => x.Permission.Code)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);
    }

    public async Task EnsureRoleAsync(
        long userId,
        string roleCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(roleCode))
            throw new ArgumentException(
                "Role code cannot be empty.",
                nameof(roleCode));

        roleCode = roleCode.Trim();

        var role = await _context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.Code == roleCode &&
                    x.IsActive,
                cancellationToken);

        if (role is null)
        {
            throw new InvalidOperationException(
                $"Role '{roleCode}' not found or inactive.");
        }

        var exists = await _context.UserRoles
            .AnyAsync(
                x =>
                    x.UserId == userId &&
                    x.RoleId == role.Id,
                cancellationToken);

        if (exists)
            return;

        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = role.Id,
            CreateDate = DateTime.UtcNow
        };

        await _context.UserRoles.AddAsync(
            userRole,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}