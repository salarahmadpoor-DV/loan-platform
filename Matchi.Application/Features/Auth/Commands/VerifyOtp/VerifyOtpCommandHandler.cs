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
    private readonly IUserRoleRepository _userRoleRepository;

public VerifyOtpCommandHandler(
    IUserRepository userRepository,
    IUserRoleRepository userRoleRepository,
    IOtpService otpService,
    IJwtTokenService jwtTokenService)
{
    _userRepository = userRepository;
    _userRoleRepository = userRoleRepository;
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
await _userRoleRepository.EnsureRoleAsync(
    user.Id,
    "USER",
    cancellationToken);
var roles = await _userRoleRepository
    .GetRoleCodesByUserIdAsync(
        user.Id,
        cancellationToken);
        var permissions = await _userRoleRepository
    .GetPermissionCodesByUserIdAsync(user.Id, cancellationToken);

var tokenResult = _jwtTokenService.GenerateToken(
    user.Id,
    user.Mobile,
    roles,
    permissions);

return new AuthResultDto
{
    AccessToken = tokenResult.AccessToken,
    RefreshToken = string.Empty,
    ExpiresAtUtc = tokenResult.ExpiresAtUtc,
    User = new AuthUserDto
    {
        Id = user.Id,
        Mobile = user.Mobile,
        Roles = roles
    }
};
    }
}
