using FluentValidation;
using FluentValidation.Results;
using Matchi.Application.Common;
using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Requests.Commands.CancelRequest;

public sealed class CancelRequestCommandHandler : IRequestHandler<CancelRequestCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRequestRepository _requestRepository;
    private readonly IDealRepository _dealRepository;

    public CancelRequestCommandHandler(
        ICurrentUserService currentUserService,
        IRequestRepository requestRepository,
        IDealRepository dealRepository)
    {
        _currentUserService = currentUserService;
        _requestRepository = requestRepository;
        _dealRepository = dealRepository;
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

        if (await _dealRepository.HasActiveDealForRequestAsync(request.Id, cancellationToken))
            throw new ConflictException("This request cannot be cancelled while it has an active deal.");

        request.Cancel();
        await _requestRepository.UpdateAsync(request, cancellationToken);
        return true;
    }
}
