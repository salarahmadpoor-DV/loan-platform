using FluentValidation;
using FluentValidation.Results;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Application.Notifications;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Executions.Commands.CreateServiceExecution;

public sealed record CreateServiceExecutionCommand(
    long DealId,
    DateOnly? ScheduledDate = null,
    TimeSpan? ScheduledTimeFrom = null,
    TimeSpan? ScheduledTimeTo = null) : IRequest<long>;

public sealed class CreateServiceExecutionCommandValidator : AbstractValidator<CreateServiceExecutionCommand>
{
    public CreateServiceExecutionCommandValidator()
    {
        RuleFor(x => x.DealId).GreaterThan(0);
        RuleFor(x => x.ScheduledTimeFrom)
            .Must((cmd, from) => from is null || cmd.ScheduledTimeTo is null || from < cmd.ScheduledTimeTo)
            .WithMessage("ScheduledTimeFrom must be earlier than ScheduledTimeTo.");
    }
}

public sealed class CreateServiceExecutionCommandHandler : IRequestHandler<CreateServiceExecutionCommand, long>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IServiceExecutionRepository _executions;
    private readonly INotificationService _notifications;
    private readonly NotificationRecipientResolver _recipients;

    public CreateServiceExecutionCommandHandler(
        ICurrentUserService currentUserService,
        IServiceExecutionRepository executions,
        INotificationService notifications,
        NotificationRecipientResolver recipients)
    {
        _currentUserService = currentUserService;
        _executions = executions;
        _notifications = notifications;
        _recipients = recipients;
    }

    public async Task<long> Handle(CreateServiceExecutionCommand command, CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        var deal = await _executions.GetActiveDealGraphAsync(command.DealId, cancellationToken);
        if (deal is null || !IsProposalParty(deal, userId))
            throw new KeyNotFoundException("Deal was not found.");

        if (!string.Equals(deal.Status, "Active", StringComparison.Ordinal))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("dealId", "An execution can only be created for an active deal.")
            });
        }

        if (deal.Request.RequestType is not ("Service" or "Hybrid"))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("dealId", "A service execution can only be created for a service or hybrid request.")
            });
        }

        if (await _executions.ExistsForDealAsync(deal.Id, cancellationToken))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("dealId", "An execution already exists for this deal.")
            });
        }

        var businessId = deal.Proposal.BusinessId;
        var execution = ServiceExecution.Create(
            deal.Id,
            businessId,
            command.ScheduledDate,
            command.ScheduledTimeFrom,
            command.ScheduledTimeTo);

        _executions.Add(execution);
        await _executions.SaveChangesAsync(cancellationToken);

        var parties = new List<long> { deal.Request.Customer.UserId };
        var originator = await _recipients.ProposalOriginatorUserIdAsync(deal.Proposal, cancellationToken);
        if (originator is > 0)
            parties.Add(originator.Value);

        await _notifications.NotifyManyAsync(
            parties,
            NotificationCatalog.ExecutionCreated(execution.Id),
            cancellationToken,
            excludeUserId: userId);

        return execution.Id;
    }

    private static bool IsProposalParty(Deal deal, long userId) =>
        (deal.Proposal.ProviderId != null && deal.Proposal.Provider!.UserId == userId)
        || (deal.Proposal.BusinessId != null && deal.Proposal.Business!.OwnerUserId == userId);
}
