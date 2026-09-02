using Matchi.Domain.Entities;

namespace Matchi.Domain.Interfaces;

public interface IBusinessRepository
{
    Task AddAsync(
        Business business,
        CancellationToken cancellationToken = default);
}
 