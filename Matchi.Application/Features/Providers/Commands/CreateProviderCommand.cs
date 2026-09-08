using FluentValidation;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Providers.Commands;

public sealed record CreateProviderCommand(
    string Name,
    double? Lat,
    double? Lng,
    string? Description = null) : IRequest<long>;

public sealed class CreateProviderCommandValidator : AbstractValidator<CreateProviderCommand>
{
    public CreateProviderCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
    }
}

public sealed class CreateProviderCommandHandler : IRequestHandler<CreateProviderCommand, long>
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

    public async Task<long> Handle(CreateProviderCommand request, CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new UnauthorizedAccessException("Authenticated user was not found.");

        var existing = await _providerRepository.GetByUserIdAsync(userId, cancellationToken);
        if (existing is not null)
            throw Fail.Validation("userId", "The authenticated user already has a provider profile.");

        var provider = new Provider(
            userId,
            request.Name,
            user.Mobile,
            request.Lat.HasValue ? (decimal)request.Lat.Value : null,
            request.Lng.HasValue ? (decimal)request.Lng.Value : null,
            request.Description);

        await _providerRepository.AddAsync(provider, cancellationToken);
        return provider.Id;
    }
}
