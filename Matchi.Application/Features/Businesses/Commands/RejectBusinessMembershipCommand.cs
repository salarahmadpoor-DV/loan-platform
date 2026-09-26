using MediatR;

namespace Matchi.Application.Features.Businesses.Commands;

public sealed record RejectBusinessMembershipCommand(long BusinessProviderId) : IRequest<bool>;
