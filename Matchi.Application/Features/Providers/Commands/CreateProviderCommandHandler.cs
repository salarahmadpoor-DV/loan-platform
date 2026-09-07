 
using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Providers.Commands;

public sealed class CreateProviderCommandHandler
    : IRequestHandler<CreateProviderCommand, long>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly IProviderRepository _providerRepository;

    public CreateProviderCommandHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IProviderRepository providerRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _providerRepository = providerRepository;
    }

    public async Task<long> Handle(
        CreateProviderCommand request,
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

        var existingProvider =
            await _providerRepository.GetByUserIdAsync(
                userId.Value,
                cancellationToken);

        if (existingProvider is not null)
            throw new InvalidOperationException(
                "The authenticated user already has a provider profile.");

        var provider = new Provider(
            userId.Value,
            request.Name,
            user.Mobile,
            request.Lat.HasValue ? (decimal)request.Lat.Value : null,
            request.Lng.HasValue ? (decimal)request.Lng.Value : null);

        await _providerRepository.AddAsync(
            provider,
            cancellationToken);

        return provider.Id;
    }
}
 
