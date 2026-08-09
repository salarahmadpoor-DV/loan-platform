using MediatR;

namespace Matchi.Application.Features.Businesses.Commands;

public sealed record AcceptBusinessMembershipCommand(long BusinessProviderId) : IRequest<bool>;
