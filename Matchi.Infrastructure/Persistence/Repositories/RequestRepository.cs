using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

public class RequestRepository : IRequestRepository
{
    private readonly MatchiDbContext _context;

    public RequestRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public async Task<long> CreateServiceRequestAsync(
        long userId,
        long serviceId,
        string title,
        string? description,
        decimal? lat,
        decimal? lng,
        IEnumerable<(long AttributeId, string? Value)> answers,
        CancellationToken cancellationToken = default)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted, cancellationToken);

        if (customer is null)
        {
            customer = new Customer(userId);
            await _context.Customers.AddAsync(customer, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var request = new Request(customer.Id, "Service", title, description);
        await _context.Requests.AddAsync(request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var requestService = new RequestService(request.Id, serviceId);
        await _context.RequestServices.AddAsync(requestService, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        if (lat.HasValue || lng.HasValue)
        {
            await _context.RequestLocations.AddAsync(
                new RequestLocation(request.Id, lat, lng),
                cancellationToken);
        }

        foreach (var answer in answers)
        {
            await _context.RequestServiceAttributes.AddAsync(
                new RequestServiceAttribute(requestService.Id, answer.AttributeId, answer.Value),
                cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return request.Id;
    }

    public async Task<Request?> GetByIdAsync(long requestId, CancellationToken cancellationToken = default)
    {
        return await _context.Requests
            .Include(r => r.Customer)
            .Include(r => r.Locations)
            .Include(r => r.Services)
                .ThenInclude(s => s.Attributes)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == requestId && !r.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Request>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _context.Requests
            .Include(r => r.Services)
            .Where(r => r.Customer.UserId == userId && !r.IsDeleted)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
