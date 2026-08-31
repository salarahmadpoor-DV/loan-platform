using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Matchi.Domain.Interfaces;

public interface IUserRoleRepository
{
    Task<IReadOnlyList<string>> GetRoleCodesByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetPermissionCodesByUserIdAsync(
    long userId,
    CancellationToken cancellationToken = default);
}
