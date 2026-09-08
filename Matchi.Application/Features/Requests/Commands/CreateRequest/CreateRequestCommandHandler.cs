using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Requests.Commands.CreateRequest;

public sealed class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, long>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly IRequestRepository _requestRepository;

    public CreateRequestCommandHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IRequestRepository requestRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _requestRepository = requestRepository;
    }

    public async Task<long> Handle(CreateRequestCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
            throw new UnauthorizedAccessException("Authenticated user was not found.");

        var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
        if (user is null)
            throw new UnauthorizedAccessException("Authenticated user was not found.");

        var customer = await _requestRepository.GetOrCreateCustomerAsync(userId.Value, cancellationToken);

        var request = new Request(
            customer.Id,
            command.RequestType,
            command.Title,
            command.Description);

        await RequestAggregateApplier.ApplyAsync(
            request,
            command.Services,
            command.Products,
            command.Location,
            command.Schedule,
            _requestRepository,
            cancellationToken);

        await _requestRepository.AddAsync(request, cancellationToken);
        return request.Id;
    }
}
