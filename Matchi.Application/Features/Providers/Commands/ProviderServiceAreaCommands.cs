using FluentValidation;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Providers.Commands;

public sealed record AddProviderServiceAreaCommand(
    string AreaType,
    string? Province,
    string? City,
    string? District,
    double? Lat,
    double? Lng,
    decimal? Radius,
    bool IsActive = true) : IRequest<long>;

public sealed class AddProviderServiceAreaCommandValidator : AbstractValidator<AddProviderServiceAreaCommand>
{
    public AddProviderServiceAreaCommandValidator()
    {
        RuleFor(x => x.AreaType).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Province).MaximumLength(100);
        RuleFor(x => x.City).MaximumLength(100);
        RuleFor(x => x.District).MaximumLength(100);
        RuleFor(x => x.Radius).GreaterThan(0).When(x => x.Radius.HasValue);
    }
}

public sealed class AddProviderServiceAreaCommandHandler : IRequestHandler<AddProviderServiceAreaCommand, long>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public AddProviderServiceAreaCommandHandler(ICurrentUserService currentUserService, IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<long> Handle(AddProviderServiceAreaCommand command, CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        CatalogRules.EnsureAreaPayload(
            command.AreaType,
            command.City,
            command.Lat.HasValue ? (decimal)command.Lat.Value : null,
            command.Lng.HasValue ? (decimal)command.Lng.Value : null,
            command.Radius);

        var area = new ProviderServiceArea(provider.Id, command.AreaType);
        area.UpdateArea(
            command.AreaType,
            command.Province,
            command.City,
            command.District,
            command.Lat.HasValue ? (decimal)command.Lat.Value : null,
            command.Lng.HasValue ? (decimal)command.Lng.Value : null,
            command.Radius,
            command.IsActive);
        await _providers.AddServiceAreaAsync(area, cancellationToken);
        return area.Id;
    }
}

public sealed record UpdateProviderServiceAreaCommand(
    long AreaId,
    string AreaType,
    string? Province,
    string? City,
    string? District,
    double? Lat,
    double? Lng,
    decimal? Radius,
    bool IsActive) : IRequest<bool>;

public sealed class UpdateProviderServiceAreaCommandValidator : AbstractValidator<UpdateProviderServiceAreaCommand>
{
    public UpdateProviderServiceAreaCommandValidator()
    {
        RuleFor(x => x.AreaId).GreaterThan(0);
        RuleFor(x => x.AreaType).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Radius).GreaterThan(0).When(x => x.Radius.HasValue);
    }
}

public sealed class UpdateProviderServiceAreaCommandHandler : IRequestHandler<UpdateProviderServiceAreaCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public UpdateProviderServiceAreaCommandHandler(ICurrentUserService currentUserService, IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<bool> Handle(UpdateProviderServiceAreaCommand command, CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        CatalogRules.EnsureAreaPayload(
            command.AreaType,
            command.City,
            command.Lat.HasValue ? (decimal)command.Lat.Value : null,
            command.Lng.HasValue ? (decimal)command.Lng.Value : null,
            command.Radius);

        var area = await _providers.GetServiceAreaAsync(provider.Id, command.AreaId, cancellationToken)
            ?? throw new KeyNotFoundException("Provider service area was not found.");

        area.UpdateArea(
            command.AreaType,
            command.Province,
            command.City,
            command.District,
            command.Lat.HasValue ? (decimal)command.Lat.Value : null,
            command.Lng.HasValue ? (decimal)command.Lng.Value : null,
            command.Radius,
            command.IsActive);
        await _providers.UpdateAsync(cancellationToken);
        return true;
    }
}

public sealed record DeleteProviderServiceAreaCommand(long AreaId) : IRequest<bool>;

public sealed class DeleteProviderServiceAreaCommandHandler : IRequestHandler<DeleteProviderServiceAreaCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public DeleteProviderServiceAreaCommandHandler(ICurrentUserService currentUserService, IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<bool> Handle(DeleteProviderServiceAreaCommand command, CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        var area = await _providers.GetServiceAreaAsync(provider.Id, command.AreaId, cancellationToken)
            ?? throw new KeyNotFoundException("Provider service area was not found.");

        area.SoftDelete();
        await _providers.UpdateAsync(cancellationToken);
        return true;
    }
}
