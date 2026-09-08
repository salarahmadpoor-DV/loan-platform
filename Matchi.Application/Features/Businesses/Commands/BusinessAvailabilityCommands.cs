using FluentValidation;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Businesses;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Businesses.Commands;

public sealed record AddBusinessAvailabilityCommand(
    long? BusinessId,
    byte DayOfWeek,
    TimeSpan TimeFrom,
    TimeSpan TimeTo,
    bool IsAvailable = true) : IRequest<long>;

public sealed class AddBusinessAvailabilityCommandValidator : AbstractValidator<AddBusinessAvailabilityCommand>
{
    public AddBusinessAvailabilityCommandValidator()
    {
        RuleFor(x => x.DayOfWeek).InclusiveBetween((byte)0, (byte)6);
        RuleFor(x => x).Must(x => x.TimeFrom < x.TimeTo)
            .WithMessage("TimeFrom must be earlier than TimeTo.");
    }
}

public sealed class AddBusinessAvailabilityCommandHandler : IRequestHandler<AddBusinessAvailabilityCommand, long>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public AddBusinessAvailabilityCommandHandler(ICurrentUserService currentUserService, IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<long> Handle(AddBusinessAvailabilityCommand command, CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, command.BusinessId, cancellationToken);
        if (command.IsAvailable &&
            await _businesses.HasAvailabilityOverlapAsync(
                business.Id, command.DayOfWeek, command.TimeFrom, command.TimeTo, null, cancellationToken))
        {
            throw Fail.Validation("timeFrom", "This availability overlaps an existing slot.");
        }

        var slot = new BusinessAvailability(business.Id, command.DayOfWeek, command.TimeFrom, command.TimeTo);
        slot.UpdateSlot(command.DayOfWeek, command.TimeFrom, command.TimeTo, command.IsAvailable);
        await _businesses.AddAvailabilityAsync(slot, cancellationToken);
        return slot.Id;
    }
}

public sealed record UpdateBusinessAvailabilityCommand(
    long? BusinessId,
    long AvailabilityId,
    byte DayOfWeek,
    TimeSpan TimeFrom,
    TimeSpan TimeTo,
    bool IsAvailable) : IRequest<bool>;

public sealed class UpdateBusinessAvailabilityCommandValidator : AbstractValidator<UpdateBusinessAvailabilityCommand>
{
    public UpdateBusinessAvailabilityCommandValidator()
    {
        RuleFor(x => x.AvailabilityId).GreaterThan(0);
        RuleFor(x => x.DayOfWeek).InclusiveBetween((byte)0, (byte)6);
        RuleFor(x => x).Must(x => x.TimeFrom < x.TimeTo)
            .WithMessage("TimeFrom must be earlier than TimeTo.");
    }
}

public sealed class UpdateBusinessAvailabilityCommandHandler : IRequestHandler<UpdateBusinessAvailabilityCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public UpdateBusinessAvailabilityCommandHandler(ICurrentUserService currentUserService, IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<bool> Handle(UpdateBusinessAvailabilityCommand command, CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, command.BusinessId, cancellationToken);
        var slot = await _businesses.GetAvailabilityAsync(business.Id, command.AvailabilityId, cancellationToken)
            ?? throw new KeyNotFoundException("Business availability was not found.");

        if (command.IsAvailable &&
            await _businesses.HasAvailabilityOverlapAsync(
                business.Id, command.DayOfWeek, command.TimeFrom, command.TimeTo, slot.Id, cancellationToken))
        {
            throw Fail.Validation("timeFrom", "This availability overlaps an existing slot.");
        }

        slot.UpdateSlot(command.DayOfWeek, command.TimeFrom, command.TimeTo, command.IsAvailable);
        await _businesses.UpdateAsync(cancellationToken);
        return true;
    }
}

public sealed record DeleteBusinessAvailabilityCommand(long? BusinessId, long AvailabilityId) : IRequest<bool>;

public sealed class DeleteBusinessAvailabilityCommandHandler : IRequestHandler<DeleteBusinessAvailabilityCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public DeleteBusinessAvailabilityCommandHandler(ICurrentUserService currentUserService, IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<bool> Handle(DeleteBusinessAvailabilityCommand command, CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, command.BusinessId, cancellationToken);
        var slot = await _businesses.GetAvailabilityAsync(business.Id, command.AvailabilityId, cancellationToken)
            ?? throw new KeyNotFoundException("Business availability was not found.");

        slot.SoftDelete();
        await _businesses.UpdateAsync(cancellationToken);
        return true;
    }
}
