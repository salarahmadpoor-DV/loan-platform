using MediatR;

namespace Matchi.Application.Features.Introductions.Commands.AssignIntroduction;

public sealed record AssignIntroductionCommand(long IntroductionId, long ProviderId) : IRequest<bool>;
