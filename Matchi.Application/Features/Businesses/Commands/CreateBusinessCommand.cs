using FluentValidation;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Businesses;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Businesses.Commands;

public sealed record CreateBusinessCommand(
    string Name,
    string? Description,
    string? Mobile,
    string? Address,
    string? Province,
    string? City,
    string? District,
    double? Lat,
    double? Lng) : IRequest<long>;

public sealed class CreateBusinessCommandValidator : AbstractValidator<CreateBusinessCommand>
{
    public CreateBusinessCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Mobile).MaximumLength(20);
        RuleFor(x => x.Address).MaximumLength(1000);
        RuleFor(x => x.Province).MaximumLength(100);
        RuleFor(x => x.City).MaximumLength(100);
        RuleFor(x => x.District).MaximumLength(100);
    }
}

public sealed class CreateBusinessCommandHandler : IRequestHandler<CreateBusinessCommand, long>
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

    public async Task<long> Handle(CreateBusinessCommand request, CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new UnauthorizedAccessException("Authenticated user was not found.");

        var business = new Business(userId, request.Name, request.Address,
            request.Lat.HasValue ? (decimal)request.Lat.Value : null,
            request.Lng.HasValue ? (decimal)request.Lng.Value : null);

        business.UpdateProfile(
            request.Name,
            request.Description,
            request.Mobile ?? user.Mobile,
            request.Address,
            request.Province,
            request.City,
            request.District,
            request.Lat.HasValue ? (decimal)request.Lat.Value : null,
            request.Lng.HasValue ? (decimal)request.Lng.Value : null,
            null);

        await _businessRepository.AddAsync(business, cancellationToken);
        return business.Id;
    }
}
