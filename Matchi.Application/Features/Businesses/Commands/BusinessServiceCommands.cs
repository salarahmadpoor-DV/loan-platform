using FluentValidation;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Businesses;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Businesses.Commands;

public sealed record AddBusinessServiceCommand(
    long? BusinessId,
    long ServiceId,
    bool IsActive = true,
    bool CanCustomerChooseProvider = false,
    decimal? MinPrice = null,
    decimal? MaxPrice = null) : IRequest<long>;

public sealed class AddBusinessServiceCommandValidator : AbstractValidator<AddBusinessServiceCommand>
{
    public AddBusinessServiceCommandValidator()
    {
        RuleFor(x => x.ServiceId).GreaterThan(0);
        RuleFor(x => x.MinPrice).GreaterThanOrEqualTo(0).When(x => x.MinPrice.HasValue);
        RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(0).When(x => x.MaxPrice.HasValue);
        RuleFor(x => x).Must(x => x.MinPrice is null || x.MaxPrice is null || x.MinPrice <= x.MaxPrice)
            .WithMessage("MinPrice must be less than or equal to MaxPrice.");
    }
}

public sealed class AddBusinessServiceCommandHandler : IRequestHandler<AddBusinessServiceCommand, long>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public AddBusinessServiceCommandHandler(ICurrentUserService currentUserService, IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<long> Handle(AddBusinessServiceCommand command, CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, command.BusinessId, cancellationToken);
        if (!await _businesses.ServiceExistsActiveAsync(command.ServiceId, cancellationToken))
            throw Fail.Validation("serviceId", "The selected service was not found or is inactive.");

        var existing = await _businesses.GetServiceLinkAsync(
            business.Id, command.ServiceId, includeDeleted: true, cancellationToken);
        if (existing is not null && !existing.IsDeleted)
            throw Fail.Validation("serviceId", "This service is already linked to the business.");

        if (existing is not null)
        {
            existing.Reactivate(command.IsActive, command.CanCustomerChooseProvider, command.MinPrice, command.MaxPrice);
            await _businesses.UpdateAsync(cancellationToken);
            return existing.Id;
        }

        var link = new BusinessService(business.Id, command.ServiceId);
        link.UpdateOffer(command.IsActive, command.CanCustomerChooseProvider, command.MinPrice, command.MaxPrice);
        await _businesses.AddServiceAsync(link, cancellationToken);
        return link.Id;
    }
}

public sealed record UpdateBusinessServiceCommand(
    long? BusinessId,
    long ServiceId,
    bool IsActive,
    bool CanCustomerChooseProvider,
    decimal? MinPrice,
    decimal? MaxPrice) : IRequest<bool>;

public sealed class UpdateBusinessServiceCommandValidator : AbstractValidator<UpdateBusinessServiceCommand>
{
    public UpdateBusinessServiceCommandValidator()
    {
        RuleFor(x => x.ServiceId).GreaterThan(0);
        RuleFor(x => x).Must(x => x.MinPrice is null || x.MaxPrice is null || x.MinPrice <= x.MaxPrice)
            .WithMessage("MinPrice must be less than or equal to MaxPrice.");
    }
}

public sealed class UpdateBusinessServiceCommandHandler : IRequestHandler<UpdateBusinessServiceCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public UpdateBusinessServiceCommandHandler(ICurrentUserService currentUserService, IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<bool> Handle(UpdateBusinessServiceCommand command, CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, command.BusinessId, cancellationToken);
        var link = await _businesses.GetServiceLinkAsync(
            business.Id, command.ServiceId, includeDeleted: false, cancellationToken)
            ?? throw new KeyNotFoundException("Business service was not found.");

        link.UpdateOffer(command.IsActive, command.CanCustomerChooseProvider, command.MinPrice, command.MaxPrice);
        await _businesses.UpdateAsync(cancellationToken);
        return true;
    }
}

public sealed record DeleteBusinessServiceCommand(long? BusinessId, long ServiceId) : IRequest<bool>;

public sealed class DeleteBusinessServiceCommandHandler : IRequestHandler<DeleteBusinessServiceCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public DeleteBusinessServiceCommandHandler(ICurrentUserService currentUserService, IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<bool> Handle(DeleteBusinessServiceCommand command, CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, command.BusinessId, cancellationToken);
        var link = await _businesses.GetServiceLinkAsync(
            business.Id, command.ServiceId, includeDeleted: false, cancellationToken)
            ?? throw new KeyNotFoundException("Business service was not found.");

        link.SoftDelete();
        await _businesses.UpdateAsync(cancellationToken);
        return true;
    }
}
