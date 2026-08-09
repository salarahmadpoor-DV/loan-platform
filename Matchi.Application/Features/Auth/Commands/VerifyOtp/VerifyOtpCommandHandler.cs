using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Auth.Commands.VerifyOtp;

public sealed class VerifyOtpCommandHandler
    : IRequestHandler<VerifyOtpCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IOtpService _otpService;
    private readonly IJwtTokenService _jwtTokenService;

    public VerifyOtpCommandHandler(
        IUserRepository userRepository,
        IOtpService otpService,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _otpService = otpService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResultDto> Handle(
        VerifyOtpCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedMobile = request.Mobile.Trim();
        var isOtpValid = await _otpService.ValidateOtpAsync(
            normalizedMobile,
            request.RequestId,
            request.Otp.Trim(),
            cancellationToken);

        if (!isOtpValid)
            throw new UnauthorizedAccessException("Invalid OTP or OTP request.");

        var existingUser = await _userRepository
            .GetByMobileAsync(normalizedMobile, cancellationToken);

        User user;

        if (existingUser is null)
        {
            user = new User(normalizedMobile);
            user.VerifyMobile();
            await _userRepository.AddAsync(user, cancellationToken);
        }
        else
        {
            user = existingUser;
            if (!user.IsMobileVerified)
            {
                user.VerifyMobile();
                await _userRepository.UpdateAsync(user, cancellationToken);
            }
        }

        var tokenResult = _jwtTokenService.GenerateToken(
            user.Id,
            user.Mobile,
            new[] { "Customer" });

        return new AuthResultDto
        {
            AccessToken = tokenResult.AccessToken,
            RefreshToken = string.Empty,
            ExpiresAtUtc = tokenResult.ExpiresAtUtc,
            User = new AuthUserDto
            {
                Id = user.Id,
                Mobile = user.Mobile,
                Roles = new[] { "Customer" }
            }
        };
    }
}
