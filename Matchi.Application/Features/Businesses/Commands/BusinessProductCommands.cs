using FluentValidation;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Businesses;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Businesses.Commands;

public sealed record AddBusinessProductCommand(
    long? BusinessId,
    long ProductId,
    decimal? Price,
    bool IsAvailable = true,
    decimal? MinOrderQuantity = null,
    int? LeadTimeDays = null) : IRequest<long>;

public sealed class AddBusinessProductCommandValidator : AbstractValidator<AddBusinessProductCommand>
{
    public AddBusinessProductCommandValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).When(x => x.Price.HasValue);
        RuleFor(x => x.MinOrderQuantity).GreaterThan(0).When(x => x.MinOrderQuantity.HasValue);
        RuleFor(x => x.LeadTimeDays).GreaterThanOrEqualTo(0).When(x => x.LeadTimeDays.HasValue);
    }
}

public sealed class AddBusinessProductCommandHandler : IRequestHandler<AddBusinessProductCommand, long>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public AddBusinessProductCommandHandler(ICurrentUserService currentUserService, IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<long> Handle(AddBusinessProductCommand command, CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, command.BusinessId, cancellationToken);
        if (!await _businesses.ProductExistsActiveAsync(command.ProductId, cancellationToken))
            throw Fail.Validation("productId", "The selected product was not found or is inactive.");

        var existing = await _businesses.GetProductLinkAsync(
            business.Id, command.ProductId, includeDeleted: true, cancellationToken);
        if (existing is not null && !existing.IsDeleted)
            throw Fail.Validation("productId", "This product is already linked to the business.");

        if (existing is not null)
        {
            existing.Reactivate(command.Price, command.IsAvailable, command.MinOrderQuantity, command.LeadTimeDays);
            await _businesses.UpdateAsync(cancellationToken);
            return existing.Id;
        }

        var link = new BusinessProduct(business.Id, command.ProductId);
        link.UpdateOffer(command.Price, command.IsAvailable, command.MinOrderQuantity, command.LeadTimeDays);
        await _businesses.AddProductAsync(link, cancellationToken);
        return link.Id;
    }
}

public sealed record UpdateBusinessProductCommand(
    long? BusinessId,
    long ProductId,
    decimal? Price,
    bool IsAvailable,
    decimal? MinOrderQuantity,
    int? LeadTimeDays) : IRequest<bool>;

public sealed class UpdateBusinessProductCommandValidator : AbstractValidator<UpdateBusinessProductCommand>
{
    public UpdateBusinessProductCommandValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).When(x => x.Price.HasValue);
        RuleFor(x => x.MinOrderQuantity).GreaterThan(0).When(x => x.MinOrderQuantity.HasValue);
        RuleFor(x => x.LeadTimeDays).GreaterThanOrEqualTo(0).When(x => x.LeadTimeDays.HasValue);
    }
}

public sealed class UpdateBusinessProductCommandHandler : IRequestHandler<UpdateBusinessProductCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public UpdateBusinessProductCommandHandler(ICurrentUserService currentUserService, IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<bool> Handle(UpdateBusinessProductCommand command, CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, command.BusinessId, cancellationToken);
        var link = await _businesses.GetProductLinkAsync(
            business.Id, command.ProductId, includeDeleted: false, cancellationToken)
            ?? throw new KeyNotFoundException("Business product was not found.");

        link.UpdateOffer(command.Price, command.IsAvailable, command.MinOrderQuantity, command.LeadTimeDays);
        await _businesses.UpdateAsync(cancellationToken);
        return true;
    }
}

public sealed record DeleteBusinessProductCommand(long? BusinessId, long ProductId) : IRequest<bool>;

public sealed class DeleteBusinessProductCommandHandler : IRequestHandler<DeleteBusinessProductCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public DeleteBusinessProductCommandHandler(ICurrentUserService currentUserService, IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<bool> Handle(DeleteBusinessProductCommand command, CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, command.BusinessId, cancellationToken);
        var link = await _businesses.GetProductLinkAsync(
            business.Id, command.ProductId, includeDeleted: false, cancellationToken)
            ?? throw new KeyNotFoundException("Business product was not found.");

        link.SoftDelete();
        await _businesses.UpdateAsync(cancellationToken);
        return true;
    }
}
