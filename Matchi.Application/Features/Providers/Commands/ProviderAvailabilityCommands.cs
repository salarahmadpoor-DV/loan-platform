using FluentValidation;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Providers.Commands;

public sealed record AddProviderAvailabilityCommand(
    byte DayOfWeek,
    TimeSpan TimeFrom,
    TimeSpan TimeTo,
    bool IsAvailable = true) : IRequest<long>;

public sealed class AddProviderAvailabilityCommandValidator : AbstractValidator<AddProviderAvailabilityCommand>
{
    public AddProviderAvailabilityCommandValidator()
    {
        RuleFor(x => x.DayOfWeek).InclusiveBetween((byte)0, (byte)6);
        RuleFor(x => x).Must(x => x.TimeFrom < x.TimeTo)
            .WithMessage("TimeFrom must be earlier than TimeTo.");
    }
}

public sealed class AddProviderAvailabilityCommandHandler : IRequestHandler<AddProviderAvailabilityCommand, long>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public AddProviderAvailabilityCommandHandler(ICurrentUserService currentUserService, IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<long> Handle(AddProviderAvailabilityCommand command, CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        if (command.IsAvailable &&
            await _providers.HasAvailabilityOverlapAsync(
                provider.Id, command.DayOfWeek, command.TimeFrom, command.TimeTo, null, cancellationToken))
        {
            throw Fail.Validation("timeFrom", "This availability overlaps an existing slot.");
        }

        var slot = new ProviderAvailability(provider.Id, command.DayOfWeek, command.TimeFrom, command.TimeTo);
        slot.UpdateSlot(command.DayOfWeek, command.TimeFrom, command.TimeTo, command.IsAvailable);
        await _providers.AddAvailabilityAsync(slot, cancellationToken);
        return slot.Id;
    }
}

public sealed record UpdateProviderAvailabilityCommand(
    long AvailabilityId,
    byte DayOfWeek,
    TimeSpan TimeFrom,
    TimeSpan TimeTo,
    bool IsAvailable) : IRequest<bool>;

public sealed class UpdateProviderAvailabilityCommandValidator : AbstractValidator<UpdateProviderAvailabilityCommand>
{
    public UpdateProviderAvailabilityCommandValidator()
    {
        RuleFor(x => x.AvailabilityId).GreaterThan(0);
        RuleFor(x => x.DayOfWeek).InclusiveBetween((byte)0, (byte)6);
        RuleFor(x => x).Must(x => x.TimeFrom < x.TimeTo)
            .WithMessage("TimeFrom must be earlier than TimeTo.");
    }
}

public sealed class UpdateProviderAvailabilityCommandHandler : IRequestHandler<UpdateProviderAvailabilityCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public UpdateProviderAvailabilityCommandHandler(ICurrentUserService currentUserService, IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<bool> Handle(UpdateProviderAvailabilityCommand command, CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        var slot = await _providers.GetAvailabilityAsync(provider.Id, command.AvailabilityId, cancellationToken)
            ?? throw new KeyNotFoundException("Provider availability was not found.");

        if (command.IsAvailable &&
            await _providers.HasAvailabilityOverlapAsync(
                provider.Id, command.DayOfWeek, command.TimeFrom, command.TimeTo, slot.Id, cancellationToken))
        {
            throw Fail.Validation("timeFrom", "This availability overlaps an existing slot.");
        }

        slot.UpdateSlot(command.DayOfWeek, command.TimeFrom, command.TimeTo, command.IsAvailable);
        await _providers.UpdateAsync(cancellationToken);
        return true;
    }
}

public sealed record DeleteProviderAvailabilityCommand(long AvailabilityId) : IRequest<bool>;

public sealed class DeleteProviderAvailabilityCommandHandler : IRequestHandler<DeleteProviderAvailabilityCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public DeleteProviderAvailabilityCommandHandler(ICurrentUserService currentUserService, IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<bool> Handle(DeleteProviderAvailabilityCommand command, CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        var slot = await _providers.GetAvailabilityAsync(provider.Id, command.AvailabilityId, cancellationToken)
            ?? throw new KeyNotFoundException("Provider availability was not found.");

        slot.SoftDelete();
        await _providers.UpdateAsync(cancellationToken);
        return true;
    }
}
