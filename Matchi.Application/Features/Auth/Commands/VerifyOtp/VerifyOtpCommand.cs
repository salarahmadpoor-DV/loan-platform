using MediatR;

namespace Matchi.Application.Features.Auth.Commands.VerifyOtp;

public sealed record VerifyOtpCommand(
    string Mobile,
    string Otp,
    string RequestId) : IRequest<AuthResultDto>;
