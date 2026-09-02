 
using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Businesses.Commands;

public sealed class CreateBusinessCommandHandler
    : IRequestHandler<CreateBusinessCommand, long>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly IBusinessRepository _businessRepository;

    public CreateBusinessCommandHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IBusinessRepository businessRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _businessRepository = businessRepository;
    }

    public async Task<long> Handle(
        CreateBusinessCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (!userId.HasValue)
            throw new UnauthorizedAccessException(
                "Authenticated user was not found.");

        var user = await _userRepository.GetByIdAsync(
            userId.Value,
            cancellationToken);

        if (user is null)
            throw new UnauthorizedAccessException(
                "Authenticated user was not found.");

        var business = new Business(
            request.Name,
            request.Address,
            request.Lat,
            request.Lng);

        business.SetOwner(userId.Value);

        await _businessRepository.AddAsync(
            business,
            cancellationToken);

        return business.Id;
    }
}
 
