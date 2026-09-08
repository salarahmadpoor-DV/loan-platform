using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Requests.Queries.GetMyRequests;

public sealed class GetMyRequestsQueryHandler : IRequestHandler<GetMyRequestsQuery, IReadOnlyList<RequestDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRequestRepository _requestRepository;

    public GetMyRequestsQueryHandler(
        ICurrentUserService currentUserService,
        IRequestRepository requestRepository)
    {
        _currentUserService = currentUserService;
        _requestRepository = requestRepository;
    }

    public async Task<IReadOnlyList<RequestDto>> Handle(
        GetMyRequestsQuery query,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
            throw new UnauthorizedAccessException("Authenticated user was not found.");

        var requests = await _requestRepository.GetByUserIdAsync(userId.Value, cancellationToken);
        return requests.Select(RequestDtoMapper.ToDto).ToList();
    }
}
