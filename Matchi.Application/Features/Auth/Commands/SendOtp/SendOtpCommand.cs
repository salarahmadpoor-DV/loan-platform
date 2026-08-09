using MediatR;

namespace Matchi.Application.Features.Auth.Commands.SendOtp;

public sealed record SendOtpCommand(string Mobile) : IRequest<string>;
