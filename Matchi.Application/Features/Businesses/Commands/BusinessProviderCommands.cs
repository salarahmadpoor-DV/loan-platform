using FluentValidation;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Businesses;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Businesses.Commands;

public sealed record AddBusinessProviderCommand(
    long? BusinessId,
    long ProviderId,
    string? Role,
    string? Status) : IRequest<long>;

public sealed class AddBusinessProviderCommandValidator : AbstractValidator<AddBusinessProviderCommand>
{
    public AddBusinessProviderCommandValidator()
    {
        RuleFor(x => x.ProviderId).GreaterThan(0);
        RuleFor(x => x.Role).MaximumLength(100);
        RuleFor(x => x.Status).MaximumLength(30);
    }
}

public sealed class AddBusinessProviderCommandHandler : IRequestHandler<AddBusinessProviderCommand, long>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public AddBusinessProviderCommandHandler(ICurrentUserService currentUserService, IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<long> Handle(AddBusinessProviderCommand command, CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, command.BusinessId, cancellationToken);

        if (!await _businesses.ProviderExistsAsync(command.ProviderId, cancellationToken))
            throw Fail.Validation("providerId", "Provider was not found.");

        var role = string.IsNullOrWhiteSpace(command.Role) ? "Member" : command.Role.Trim();
        var status = string.IsNullOrWhiteSpace(command.Status) ? "Active" : command.Status.Trim();
        if (!CatalogRules.MembershipStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
            throw Fail.Validation("status", "Status must be Active, Inactive, or Pending.");

        var existing = await _businesses.GetMembershipAsync(
            business.Id, command.ProviderId, includeDeleted: true, cancellationToken);

        if (existing is not null && !existing.IsDeleted)
            throw Fail.Validation("providerId", "This provider is already a member of the business.");

        if (existing is not null)
        {
            existing.Reactivate(role);
            existing.UpdateMembership(role, status);
            await _businesses.UpdateAsync(cancellationToken);
            return existing.Id;
        }

        var membership = new BusinessProvider(business.Id, command.ProviderId, role);
        membership.UpdateMembership(role, status);
        await _businesses.AddMembershipAsync(membership, cancellationToken);
        return membership.Id;
    }
}

public sealed record UpdateBusinessProviderCommand(
    long? BusinessId,
    long ProviderId,
    string Role,
    string Status) : IRequest<bool>;

public sealed class UpdateBusinessProviderCommandValidator : AbstractValidator<UpdateBusinessProviderCommand>
{
    public UpdateBusinessProviderCommandValidator()
    {
        RuleFor(x => x.ProviderId).GreaterThan(0);
        RuleFor(x => x.Role).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Status).NotEmpty().MaximumLength(30);
    }
}

public sealed class UpdateBusinessProviderCommandHandler : IRequestHandler<UpdateBusinessProviderCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public UpdateBusinessProviderCommandHandler(ICurrentUserService currentUserService, IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<bool> Handle(UpdateBusinessProviderCommand command, CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, command.BusinessId, cancellationToken);

        if (!CatalogRules.MembershipStatuses.Contains(command.Status, StringComparer.OrdinalIgnoreCase))
            throw Fail.Validation("status", "Status must be Active, Inactive, or Pending.");

        var membership = await _businesses.GetMembershipAsync(
            business.Id, command.ProviderId, includeDeleted: false, cancellationToken)
            ?? throw new KeyNotFoundException("Business provider membership was not found.");

        membership.UpdateMembership(command.Role, command.Status);
        await _businesses.UpdateAsync(cancellationToken);
        return true;
    }
}

public sealed record DeleteBusinessProviderCommand(long? BusinessId, long ProviderId) : IRequest<bool>;

public sealed class DeleteBusinessProviderCommandHandler : IRequestHandler<DeleteBusinessProviderCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public DeleteBusinessProviderCommandHandler(ICurrentUserService currentUserService, IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<bool> Handle(DeleteBusinessProviderCommand command, CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, command.BusinessId, cancellationToken);
        var membership = await _businesses.GetMembershipAsync(
            business.Id, command.ProviderId, includeDeleted: false, cancellationToken)
            ?? throw new KeyNotFoundException("Business provider membership was not found.");

        membership.Leave();
        await _businesses.UpdateAsync(cancellationToken);
        return true;
    }
}
