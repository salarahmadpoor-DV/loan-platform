using MediatR;

namespace Matchi.Application.Features.Introductions.Commands.ConfirmIntroduction;

public sealed record ConfirmIntroductionCommand(long IntroductionId, string Status) : IRequest<bool>;
