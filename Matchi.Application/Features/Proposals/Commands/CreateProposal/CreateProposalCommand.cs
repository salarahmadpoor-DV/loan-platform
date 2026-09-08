using MediatR;
using Matchi.Application.Features.Proposals;

namespace Matchi.Application.Features.Proposals.Commands.CreateProposal;

public sealed record CreateProposalCommand(
    long RequestId,
    string ProposerType,
    decimal TotalPrice,
    decimal DeliveryFee = 0m,
    string? Message = null,
    DateOnly? ProposedDate = null,
    TimeSpan? ProposedTimeFrom = null,
    TimeSpan? ProposedTimeTo = null,
    DateTime? ExpireAt = null,
    long? BusinessId = null,
    IReadOnlyList<CreateProposalItemDto>? Items = null) : IRequest<long>;
