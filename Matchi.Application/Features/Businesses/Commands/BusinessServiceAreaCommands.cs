using FluentValidation;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Businesses;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Businesses.Commands;

public sealed record AddBusinessServiceAreaCommand(
    long? BusinessId,
    string AreaType,
    string? Province,
    string? City,
    string? District,
    double? Lat,
    double? Lng,
    decimal? Radius,
    bool IsActive = true) : IRequest<long>;

public sealed class AddBusinessServiceAreaCommandValidator : AbstractValidator<AddBusinessServiceAreaCommand>
{
    public AddBusinessServiceAreaCommandValidator()
    {
        RuleFor(x => x.AreaType).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Radius).GreaterThan(0).When(x => x.Radius.HasValue);
    }
}

public sealed class AddBusinessServiceAreaCommandHandler : IRequestHandler<AddBusinessServiceAreaCommand, long>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public AddBusinessServiceAreaCommandHandler(ICurrentUserService currentUserService, IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<long> Handle(AddBusinessServiceAreaCommand command, CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, command.BusinessId, cancellationToken);
        CatalogRules.EnsureAreaPayload(
            command.AreaType,
            command.City,
            command.Lat.HasValue ? (decimal)command.Lat.Value : null,
            command.Lng.HasValue ? (decimal)command.Lng.Value : null,
            command.Radius);

        var area = new BusinessServiceArea(business.Id, command.AreaType);
        area.UpdateArea(
            command.AreaType,
            command.Province,
            command.City,
            command.District,
            command.Lat.HasValue ? (decimal)command.Lat.Value : null,
            command.Lng.HasValue ? (decimal)command.Lng.Value : null,
            command.Radius,
            command.IsActive);
        await _businesses.AddServiceAreaAsync(area, cancellationToken);
        return area.Id;
    }
}

public sealed record UpdateBusinessServiceAreaCommand(
    long? BusinessId,
    long AreaId,
    string AreaType,
    string? Province,
    string? City,
    string? District,
    double? Lat,
    double? Lng,
    decimal? Radius,
    bool IsActive) : IRequest<bool>;

public sealed class UpdateBusinessServiceAreaCommandValidator : AbstractValidator<UpdateBusinessServiceAreaCommand>
{
    public UpdateBusinessServiceAreaCommandValidator()
    {
        RuleFor(x => x.AreaId).GreaterThan(0);
        RuleFor(x => x.AreaType).NotEmpty().MaximumLength(30);
    }
}

public sealed class UpdateBusinessServiceAreaCommandHandler : IRequestHandler<UpdateBusinessServiceAreaCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public UpdateBusinessServiceAreaCommandHandler(ICurrentUserService currentUserService, IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<bool> Handle(UpdateBusinessServiceAreaCommand command, CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, command.BusinessId, cancellationToken);
        CatalogRules.EnsureAreaPayload(
            command.AreaType,
            command.City,
            command.Lat.HasValue ? (decimal)command.Lat.Value : null,
            command.Lng.HasValue ? (decimal)command.Lng.Value : null,
            command.Radius);

        var area = await _businesses.GetServiceAreaAsync(business.Id, command.AreaId, cancellationToken)
            ?? throw new KeyNotFoundException("Business service area was not found.");

        area.UpdateArea(
            command.AreaType,
            command.Province,
            command.City,
            command.District,
            command.Lat.HasValue ? (decimal)command.Lat.Value : null,
            command.Lng.HasValue ? (decimal)command.Lng.Value : null,
            command.Radius,
            command.IsActive);
        await _businesses.UpdateAsync(cancellationToken);
        return true;
    }
}

public sealed record DeleteBusinessServiceAreaCommand(long? BusinessId, long AreaId) : IRequest<bool>;

public sealed class DeleteBusinessServiceAreaCommandHandler : IRequestHandler<DeleteBusinessServiceAreaCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public DeleteBusinessServiceAreaCommandHandler(ICurrentUserService currentUserService, IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<bool> Handle(DeleteBusinessServiceAreaCommand command, CancellationToken cancellationToken)
    {
        var business = await BusinessAccess.RequireOwned(
            _currentUserService, _businesses, command.BusinessId, cancellationToken);
        var area = await _businesses.GetServiceAreaAsync(business.Id, command.AreaId, cancellationToken)
            ?? throw new KeyNotFoundException("Business service area was not found.");

        area.SoftDelete();
        await _businesses.UpdateAsync(cancellationToken);
        return true;
    }
}
