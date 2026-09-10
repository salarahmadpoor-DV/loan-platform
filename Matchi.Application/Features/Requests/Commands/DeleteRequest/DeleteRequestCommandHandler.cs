using Matchi.Application.Common;
using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Requests.Commands.DeleteRequest;

public sealed class DeleteRequestCommandHandler : IRequestHandler<DeleteRequestCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRequestRepository _requestRepository;
    private readonly IDealRepository _dealRepository;

    public DeleteRequestCommandHandler(
        ICurrentUserService currentUserService,
        IRequestRepository requestRepository,
        IDealRepository dealRepository)
    {
        _currentUserService = currentUserService;
        _requestRepository = requestRepository;
        _dealRepository = dealRepository;
    }

    public async Task<bool> Handle(DeleteRequestCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
            throw new UnauthorizedAccessException("Authenticated user was not found.");

        var request = await _requestRepository.GetOwnedByIdAsync(
            command.RequestId,
            userId.Value,
            cancellationToken);

        if (request is null)
            throw new KeyNotFoundException("Request was not found.");

        if (await _dealRepository.HasActiveDealForRequestAsync(request.Id, cancellationToken))
            throw new ConflictException("This request cannot be deleted while it has an active deal.");

        request.SoftDelete();
        await _requestRepository.UpdateAsync(request, cancellationToken);
        return true;
    }
}
