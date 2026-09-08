using FluentValidation;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Providers.Commands;

public sealed record AddProviderProductCommand(
    long ProductId,
    decimal? Price,
    bool IsAvailable = true,
    decimal? MinOrderQuantity = null,
    int? LeadTimeDays = null) : IRequest<long>;

public sealed class AddProviderProductCommandValidator : AbstractValidator<AddProviderProductCommand>
{
    public AddProviderProductCommandValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).When(x => x.Price.HasValue);
        RuleFor(x => x.MinOrderQuantity).GreaterThan(0).When(x => x.MinOrderQuantity.HasValue);
        RuleFor(x => x.LeadTimeDays).GreaterThanOrEqualTo(0).When(x => x.LeadTimeDays.HasValue);
    }
}

public sealed class AddProviderProductCommandHandler : IRequestHandler<AddProviderProductCommand, long>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public AddProviderProductCommandHandler(ICurrentUserService currentUserService, IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<long> Handle(AddProviderProductCommand command, CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        if (!await _providers.ProductExistsActiveAsync(command.ProductId, cancellationToken))
            throw Fail.Validation("productId", "The selected product was not found or is inactive.");

        var existing = await _providers.GetProductLinkAsync(
            provider.Id, command.ProductId, includeDeleted: true, cancellationToken);

        if (existing is not null && !existing.IsDeleted)
            throw Fail.Validation("productId", "This product is already linked to the provider.");

        if (existing is not null)
        {
            existing.Reactivate(command.Price, command.IsAvailable, command.MinOrderQuantity, command.LeadTimeDays);
            await _providers.UpdateAsync(cancellationToken);
            return existing.Id;
        }

        var link = new ProviderProduct(provider.Id, command.ProductId);
        link.UpdateOffer(command.Price, command.IsAvailable, command.MinOrderQuantity, command.LeadTimeDays);
        await _providers.AddProductAsync(link, cancellationToken);
        return link.Id;
    }
}

public sealed record UpdateProviderProductCommand(
    long ProductId,
    decimal? Price,
    bool IsAvailable,
    decimal? MinOrderQuantity,
    int? LeadTimeDays) : IRequest<bool>;

public sealed class UpdateProviderProductCommandValidator : AbstractValidator<UpdateProviderProductCommand>
{
    public UpdateProviderProductCommandValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).When(x => x.Price.HasValue);
        RuleFor(x => x.MinOrderQuantity).GreaterThan(0).When(x => x.MinOrderQuantity.HasValue);
        RuleFor(x => x.LeadTimeDays).GreaterThanOrEqualTo(0).When(x => x.LeadTimeDays.HasValue);
    }
}

public sealed class UpdateProviderProductCommandHandler : IRequestHandler<UpdateProviderProductCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public UpdateProviderProductCommandHandler(ICurrentUserService currentUserService, IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<bool> Handle(UpdateProviderProductCommand command, CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        var link = await _providers.GetProductLinkAsync(
            provider.Id, command.ProductId, includeDeleted: false, cancellationToken)
            ?? throw new KeyNotFoundException("Provider product was not found.");

        link.UpdateOffer(command.Price, command.IsAvailable, command.MinOrderQuantity, command.LeadTimeDays);
        await _providers.UpdateAsync(cancellationToken);
        return true;
    }
}

public sealed record DeleteProviderProductCommand(long ProductId) : IRequest<bool>;

public sealed class DeleteProviderProductCommandHandler : IRequestHandler<DeleteProviderProductCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;

    public DeleteProviderProductCommandHandler(ICurrentUserService currentUserService, IProviderRepository providers)
    {
        _currentUserService = currentUserService;
        _providers = providers;
    }

    public async Task<bool> Handle(DeleteProviderProductCommand command, CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        var link = await _providers.GetProductLinkAsync(
            provider.Id, command.ProductId, includeDeleted: false, cancellationToken)
            ?? throw new KeyNotFoundException("Provider product was not found.");

        link.SoftDelete();
        await _providers.UpdateAsync(cancellationToken);
        return true;
    }
}
