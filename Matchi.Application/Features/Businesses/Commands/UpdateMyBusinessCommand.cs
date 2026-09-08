using FluentValidation;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Businesses;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Businesses.Commands;

public sealed record UpdateMyBusinessCommand(
    long? BusinessId,
    string Name,
    string? Description,
    string? Mobile,
    string? Address,
    string? Province,
    string? City,
    string? District,
    double? Lat,
    double? Lng,
    long? LogoMediaId) : IRequest<bool>;

public sealed class UpdateMyBusinessCommandValidator : AbstractValidator<UpdateMyBusinessCommand>
{
    public UpdateMyBusinessCommandValidator()
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

public sealed class UpdateMyBusinessCommandHandler : IRequestHandler<UpdateMyBusinessCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public UpdateMyBusinessCommandHandler(
        ICurrentUserService currentUserService,
        IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<bool> Handle(UpdateMyBusinessCommand command, CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, command.BusinessId, cancellationToken);

        business.UpdateProfile(
            command.Name,
            command.Description,
            command.Mobile,
            command.Address,
            command.Province,
            command.City,
            command.District,
            command.Lat.HasValue ? (decimal)command.Lat.Value : null,
            command.Lng.HasValue ? (decimal)command.Lng.Value : null,
            command.LogoMediaId);

        await _businesses.UpdateAsync(cancellationToken);
        return true;
    }
}
