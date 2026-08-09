using FluentValidation;

namespace Matchi.Application.Features.Auth.Commands.VerifyOtp;

public sealed class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
    {
        RuleFor(x => x.Mobile)
            .NotEmpty().WithMessage("Mobile phone number is required.")
            .Matches("^\\+?\\d{8,15}$").WithMessage("Mobile phone number must contain 8 to 15 digits.");

        RuleFor(x => x.Otp)
            .NotEmpty().WithMessage("OTP code is required.")
            .Matches("^\\d{4,8}$").WithMessage("OTP code must contain 4 to 8 digits.");

        RuleFor(x => x.RequestId)
            .NotEmpty().WithMessage("OTP request id is required.");
    }
}
