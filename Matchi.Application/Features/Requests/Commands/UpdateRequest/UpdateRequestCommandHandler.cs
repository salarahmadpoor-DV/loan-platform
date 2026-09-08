using FluentValidation;
using FluentValidation.Results;
using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Requests.Commands.UpdateRequest;

public sealed class UpdateRequestCommandHandler : IRequestHandler<UpdateRequestCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRequestRepository _requestRepository;

    public UpdateRequestCommandHandler(
        ICurrentUserService currentUserService,
        IRequestRepository requestRepository)
    {
        _currentUserService = currentUserService;
        _requestRepository = requestRepository;
    }

    public async Task<bool> Handle(UpdateRequestCommand command, CancellationToken cancellationToken)
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
                new ValidationFailure("requestId", "Only an open request owned by the current user can be updated.")
            });
        }

        request.UpdateDetails(
            command.Body.RequestType,
            command.Body.Title,
            command.Body.Description);

        request.RetireLineItems();
        _requestRepository.RemoveLocationsAndSchedules(request);

        await RequestAggregateApplier.ApplyAsync(
            request,
            command.Body.Services,
            command.Body.Products,
            command.Body.Location,
            command.Body.Schedule,
            _requestRepository,
            cancellationToken);

        await _requestRepository.UpdateAsync(request, cancellationToken);
        return true;
    }
}
