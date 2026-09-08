using FluentValidation;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Providers.Commands;

public sealed record AddProviderServiceCommand(long ServiceId) : IRequest<long>;

public sealed class AddProviderServiceCommandValidator : AbstractValidator<AddProviderServiceCommand>
{
    public AddProviderServiceCommandValidator()
    {
        RuleFor(x => x.ServiceId).GreaterThan(0);
    }
}

public sealed class AddProviderServiceCommandHandler : IRequestHandler<AddProviderServiceCommand, long>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public AddProviderServiceCommandHandler(ICurrentUserService currentUserService, IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<long> Handle(AddProviderServiceCommand command, CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        if (!await _providers.ServiceExistsActiveAsync(command.ServiceId, cancellationToken))
            throw Fail.Validation("serviceId", "The selected service was not found or is inactive.");

        var existing = await _providers.GetServiceLinkAsync(
            provider.Id, command.ServiceId, includeDeleted: true, cancellationToken);

        if (existing is not null && !existing.IsDeleted)
            throw Fail.Validation("serviceId", "This service is already linked to the provider.");

        if (existing is not null)
        {
            existing.Reactivate();
            await _providers.UpdateAsync(cancellationToken);
            return existing.Id;
        }

        var link = new ProviderService(provider.Id, command.ServiceId);
        await _providers.AddServiceAsync(link, cancellationToken);
        return link.Id;
    }
}

public sealed record UpdateProviderServiceCommand(long ServiceId, bool IsActive) : IRequest<bool>;

public sealed class UpdateProviderServiceCommandHandler : IRequestHandler<UpdateProviderServiceCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public UpdateProviderServiceCommandHandler(ICurrentUserService currentUserService, IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<bool> Handle(UpdateProviderServiceCommand command, CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        var link = await _providers.GetServiceLinkAsync(
            provider.Id, command.ServiceId, includeDeleted: false, cancellationToken)
            ?? throw new KeyNotFoundException("Provider service was not found.");

        link.SetActive(command.IsActive);
        await _providers.UpdateAsync(cancellationToken);
        return true;
    }
}

public sealed record DeleteProviderServiceCommand(long ServiceId) : IRequest<bool>;

public sealed class DeleteProviderServiceCommandHandler : IRequestHandler<DeleteProviderServiceCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public DeleteProviderServiceCommandHandler(ICurrentUserService currentUserService, IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<bool> Handle(DeleteProviderServiceCommand command, CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        var link = await _providers.GetServiceLinkAsync(
            provider.Id, command.ServiceId, includeDeleted: false, cancellationToken)
            ?? throw new KeyNotFoundException("Provider service was not found.");

        link.SoftDelete();
        await _providers.UpdateAsync(cancellationToken);
        return true;
    }
}
