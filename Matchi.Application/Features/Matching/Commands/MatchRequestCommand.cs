using MediatR;

namespace Matchi.Application.Features.Matching.Commands;

public sealed record MatchRequestCommand(Guid RequestId, string? Method = "basic") : IRequest<MatchResultDto>;
