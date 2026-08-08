using FluentValidation;

namespace Matchi.Application.Features.LoanRequests.Commands.CreateLoanRequest;

public sealed class CreateLoanRequestValidator
    : AbstractValidator<CreateLoanRequestCommand>
{
    public CreateLoanRequestValidator()
    {
        RuleFor(x => x.BankId)
            .GreaterThan(0);

        RuleFor(x => x.UserId)
            .GreaterThan(0);
    }
}