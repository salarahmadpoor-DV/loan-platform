using FluentValidation;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Providers.Commands;

public sealed record AddProviderCapabilityCommand(long ServiceAttributeId, string Value) : IRequest<long>;

public sealed class AddProviderCapabilityCommandValidator : AbstractValidator<AddProviderCapabilityCommand>
{
    public AddProviderCapabilityCommandValidator()
    {
        RuleFor(x => x.ServiceAttributeId).GreaterThan(0);
        RuleFor(x => x.Value).NotEmpty().MaximumLength(500);
    }
}

public sealed class AddProviderCapabilityCommandHandler : IRequestHandler<AddProviderCapabilityCommand, long>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public AddProviderCapabilityCommandHandler(ICurrentUserService currentUserService, IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<long> Handle(AddProviderCapabilityCommand command, CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        var attribute = await _providers.GetServiceAttributeAsync(command.ServiceAttributeId, cancellationToken)
            ?? throw Fail.Validation("serviceAttributeId", "Service attribute was not found or is inactive.");

        if (!await _providers.OffersServiceAsync(provider.Id, attribute.ServiceId, cancellationToken))
            throw Fail.Validation("serviceAttributeId", "Capability must belong to a service offered by the provider.");

        var existing = await _providers.GetCapabilityAsync(
            provider.Id, command.ServiceAttributeId, includeDeleted: true, cancellationToken);

        if (existing is not null && !existing.IsDeleted)
            throw Fail.Validation("serviceAttributeId", "This capability is already defined for the provider.");

        if (existing is not null)
        {
            existing.Reactivate(command.Value);
            await _providers.UpdateAsync(cancellationToken);
            return existing.Id;
        }

        var capability = new ProviderCapability(provider.Id, command.ServiceAttributeId, command.Value);
        await _providers.AddCapabilityAsync(capability, cancellationToken);
        return capability.Id;
    }
}

public sealed record UpdateProviderCapabilityCommand(long ServiceAttributeId, string Value) : IRequest<bool>;

public sealed class UpdateProviderCapabilityCommandValidator : AbstractValidator<UpdateProviderCapabilityCommand>
{
    public UpdateProviderCapabilityCommandValidator()
    {
        RuleFor(x => x.ServiceAttributeId).GreaterThan(0);
        RuleFor(x => x.Value).NotEmpty().MaximumLength(500);
    }
}

public sealed class UpdateProviderCapabilityCommandHandler : IRequestHandler<UpdateProviderCapabilityCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public UpdateProviderCapabilityCommandHandler(ICurrentUserService currentUserService, IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<bool> Handle(UpdateProviderCapabilityCommand command, CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        var capability = await _providers.GetCapabilityAsync(
            provider.Id, command.ServiceAttributeId, includeDeleted: false, cancellationToken)
            ?? throw new KeyNotFoundException("Provider capability was not found.");

        capability.UpdateValue(command.Value);
        await _providers.UpdateAsync(cancellationToken);
        return true;
    }
}

public sealed record DeleteProviderCapabilityCommand(long ServiceAttributeId) : IRequest<bool>;

public sealed class DeleteProviderCapabilityCommandHandler : IRequestHandler<DeleteProviderCapabilityCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public DeleteProviderCapabilityCommandHandler(ICurrentUserService currentUserService, IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<bool> Handle(DeleteProviderCapabilityCommand command, CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        var capability = await _providers.GetCapabilityAsync(
            provider.Id, command.ServiceAttributeId, includeDeleted: false, cancellationToken)
            ?? throw new KeyNotFoundException("Provider capability was not found.");

        capability.SoftDelete();
        await _providers.UpdateAsync(cancellationToken);
        return true;
    }
}
