using FluentValidation;
using MediatR;

namespace Matchi.Application.Features.Businesses.Commands;

public sealed record InviteProviderCommand(
    long BusinessId,
    long? ProviderId,
    string? Mobile,
    string? Role) : IRequest<long>;

public sealed class InviteProviderCommandValidator : AbstractValidator<InviteProviderCommand>
{
    public InviteProviderCommandValidator()
    {
        RuleFor(x => x.BusinessId).GreaterThan(0);
        RuleFor(x => x.Role).MaximumLength(100);
        RuleFor(x => x.Mobile).MaximumLength(20);
        RuleFor(x => x)
            .Must(x => x.ProviderId is > 0 || !string.IsNullOrWhiteSpace(x.Mobile))
            .WithMessage("Provide providerId or mobile.");
    }
}
