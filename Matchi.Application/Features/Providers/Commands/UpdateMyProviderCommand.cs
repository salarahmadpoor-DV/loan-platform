using FluentValidation;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Providers.Commands;

public sealed record UpdateMyProviderCommand(
    string Name,
    string? Description,
    string? Mobile,
    double? Lat,
    double? Lng) : IRequest<bool>;

public sealed class UpdateMyProviderCommandValidator : AbstractValidator<UpdateMyProviderCommand>
{
    public UpdateMyProviderCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Mobile).MaximumLength(20);
    }
}

public sealed class UpdateMyProviderCommandHandler : IRequestHandler<UpdateMyProviderCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public UpdateMyProviderCommandHandler(
        ICurrentUserService currentUserService,
        IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<bool> Handle(UpdateMyProviderCommand command, CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        provider.UpdateProfile(
            command.Name,
            command.Description,
            command.Mobile,
            command.Lat.HasValue ? (decimal)command.Lat.Value : null,
            command.Lng.HasValue ? (decimal)command.Lng.Value : null);
        await _providers.UpdateAsync(cancellationToken);
        return true;
    }
}
