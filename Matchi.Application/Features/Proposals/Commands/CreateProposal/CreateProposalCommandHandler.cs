using FluentValidation;
using FluentValidation.Results;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Businesses;
using Matchi.Application.Features.Proposals;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Proposals.Commands.CreateProposal;

public sealed class CreateProposalCommandHandler : IRequestHandler<CreateProposalCommand, long>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRequestRepository _requestRepository;
    private readonly IProviderRepository _providerRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly IProposalRepository _proposalRepository;

    public CreateProposalCommandHandler(
        ICurrentUserService currentUserService,
        IRequestRepository requestRepository,
        IProviderRepository providerRepository,
        IBusinessRepository businessRepository,
        IProposalRepository proposalRepository)
    {
        _currentUserService = currentUserService;
        _requestRepository = requestRepository;
        _providerRepository = providerRepository;
        _businessRepository = businessRepository;
        _proposalRepository = proposalRepository;
    }

    public async Task<long> Handle(CreateProposalCommand command, CancellationToken cancellationToken)
    {
        _ = Actor.RequireUserId(_currentUserService);

        var request = await _requestRepository.GetByIdAsync(command.RequestId, cancellationToken);
        if (request is null)
            throw new KeyNotFoundException("Request was not found.");

        if (!string.Equals(request.Status, "Open", StringComparison.Ordinal))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("requestId", "A proposal can only be created for an open request.")
            });
        }

        var items = command.Items ?? [];
        var asProvider = command.ProposerType == "Provider";

        Proposal proposal;
        long partyId;

        if (asProvider)
        {
            var provider = await ProviderAccess.RequireMine(
                _currentUserService,
                _providerRepository,
                cancellationToken);
            partyId = provider.Id;
            await EnsureCatalogAsync(true, partyId, items, cancellationToken);
            proposal = Proposal.ForProvider(
                command.RequestId,
                provider.Id,
                command.TotalPrice,
                command.DeliveryFee,
                command.Message,
                command.ProposedDate,
                command.ProposedTimeFrom,
                command.ProposedTimeTo,
                command.ExpireAt);
        }
        else
        {
            var business = await BusinessAccess.RequireOwned(
                _currentUserService,
                _businessRepository,
                command.BusinessId,
                cancellationToken);
            partyId = business.Id;
            await EnsureCatalogAsync(false, partyId, items, cancellationToken);
            proposal = Proposal.ForBusiness(
                command.RequestId,
                business.Id,
                command.TotalPrice,
                command.DeliveryFee,
                command.Message,
                command.ProposedDate,
                command.ProposedTimeFrom,
                command.ProposedTimeTo,
                command.ExpireAt);
        }

        foreach (var item in items)
        {
            proposal.AddItem(item.ItemType == "Service"
                ? ProposalItem.ForService(
                    item.ServiceId!.Value,
                    item.Quantity,
                    item.UnitPrice,
                    item.TotalPrice,
                    item.Description,
                    item.DisplayOrder)
                : ProposalItem.ForProduct(
                    item.ProductId!.Value,
                    item.Quantity,
                    item.UnitPrice,
                    item.TotalPrice,
                    item.Description,
                    item.DisplayOrder));
        }

        await _proposalRepository.AddAsync(proposal, cancellationToken);
        return proposal.Id;
    }

    private async Task EnsureCatalogAsync(
        bool asProvider,
        long partyId,
        IReadOnlyList<CreateProposalItemDto> items,
        CancellationToken cancellationToken)
    {
        var serviceIds = items
            .Where(i => i.ItemType == "Service" && i.ServiceId.HasValue)
            .Select(i => i.ServiceId!.Value)
            .Distinct()
            .ToList();
        var productIds = items
            .Where(i => i.ItemType == "Product" && i.ProductId.HasValue)
            .Select(i => i.ProductId!.Value)
            .Distinct()
            .ToList();

        if (serviceIds.Count > 0)
        {
            var active = await _proposalRepository.GetActiveServiceIdsAsync(serviceIds, cancellationToken);
            var missing = serviceIds.Except(active).ToList();
            if (missing.Count > 0)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("items", $"Service {missing[0]} was not found or is not active.")
                });
            }

            var offered = await _proposalRepository.GetOfferedServiceIdsAsync(
                asProvider,
                partyId,
                serviceIds,
                cancellationToken);
            missing = serviceIds.Except(offered).ToList();
            if (missing.Count > 0)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("items", $"Service {missing[0]} is not offered by the proposing party.")
                });
            }
        }

        if (productIds.Count > 0)
        {
            var active = await _proposalRepository.GetActiveProductIdsAsync(productIds, cancellationToken);
            var missing = productIds.Except(active).ToList();
            if (missing.Count > 0)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("items", $"Product {missing[0]} was not found or is not active.")
                });
            }

            var offered = await _proposalRepository.GetOfferedProductIdsAsync(
                asProvider,
                partyId,
                productIds,
                cancellationToken);
            missing = productIds.Except(offered).ToList();
            if (missing.Count > 0)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("items", $"Product {missing[0]} is not offered by the proposing party.")
                });
            }
        }
    }
}
