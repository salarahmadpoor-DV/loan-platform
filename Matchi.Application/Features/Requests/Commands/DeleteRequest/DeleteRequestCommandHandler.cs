using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Requests.Commands.DeleteRequest;

public sealed class DeleteRequestCommandHandler : IRequestHandler<DeleteRequestCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRequestRepository _requestRepository;

    public DeleteRequestCommandHandler(
        ICurrentUserService currentUserService,
        IRequestRepository requestRepository)
    {
        _currentUserService = currentUserService;
        _requestRepository = requestRepository;
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

        request.SoftDelete();
        await _requestRepository.UpdateAsync(request, cancellationToken);
        return true;
    }
}
