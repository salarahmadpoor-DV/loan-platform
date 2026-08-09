using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

public class ServiceRequestRepository : IServiceRequestRepository
{
    private readonly MatchiDbContext _context;

    public ServiceRequestRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public async Task<long> CreateRequestAsync(ServiceRequest request, IEnumerable<RequestAnswer> answers, CancellationToken cancellationToken = default)
    {
        await _context.ServiceRequests.AddAsync(request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken); // get request.Id

        foreach (var a in answers ?? Enumerable.Empty<RequestAnswer>())
        {
            // create new answer linked to saved request
            var ans = new RequestAnswer(request.Id, a.ServiceQuestionId, a.SelectedOptionId, a.Text);
            await _context.RequestAnswers.AddAsync(ans, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return request.Id;
    }

    public async Task<ServiceRequest?> GetByIdAsync(long requestId, CancellationToken cancellationToken = default)
    {
        return await _context.ServiceRequests
            .Include(r => r.Answers)
            .Include(r => r.Introductions)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);
    }

    public async Task<IEnumerable<ServiceRequest>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _context.ServiceRequests
            .Where(r => r.UserId == userId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}