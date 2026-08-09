using FluentValidation;

namespace Matchi.Application.Features.Auth.Commands.SendOtp;

public sealed class SendOtpCommandValidator : AbstractValidator<SendOtpCommand>
{
    public SendOtpCommandValidator()
    {
        RuleFor(x => x.Mobile)
            .NotEmpty().WithMessage("Mobile phone number is required.")
            .Matches("^\\+?\\d{8,15}$").WithMessage("Mobile phone number must contain 8 to 15 digits.");
    }
}
