using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Requests.Queries.GetRequestById;

public sealed class GetRequestByIdQueryHandler : IRequestHandler<GetRequestByIdQuery, RequestDto?>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRequestRepository _requestRepository;

    public GetRequestByIdQueryHandler(
        ICurrentUserService currentUserService,
        IRequestRepository requestRepository)
    {
        _currentUserService = currentUserService;
        _requestRepository = requestRepository;
    }

    public async Task<RequestDto?> Handle(GetRequestByIdQuery query, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
            throw new UnauthorizedAccessException("Authenticated user was not found.");

        var request = await _requestRepository.GetOwnedByIdAsync(
            query.RequestId,
            userId.Value,
            cancellationToken);

        return request is null ? null : RequestDtoMapper.ToDto(request);
    }
}
