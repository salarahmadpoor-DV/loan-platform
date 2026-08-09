using MediatR;

namespace Matchi.Application.Features.Introductions.Commands;

public sealed record CreateIntroductionCommand(long RequestId, string TargetType, long TargetId, string? Source) : IRequest<long>;
