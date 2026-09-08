using FluentValidation;
using FluentValidation.Results;
using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Requests.Commands.CancelRequest;

public sealed class CancelRequestCommandHandler : IRequestHandler<CancelRequestCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRequestRepository _requestRepository;

    public CancelRequestCommandHandler(
        ICurrentUserService currentUserService,
        IRequestRepository requestRepository)
    {
        _currentUserService = currentUserService;
        _requestRepository = requestRepository;
    }

    public async Task<bool> Handle(CancelRequestCommand command, CancellationToken cancellationToken)
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

        if (!request.CanBeModified())
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("requestId", "Only an open request owned by the current user can be cancelled.")
            });
        }

        request.Cancel();
        await _requestRepository.UpdateAsync(request, cancellationToken);
        return true;
    }
}
