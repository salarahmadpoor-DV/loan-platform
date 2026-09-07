using MediatR;

namespace Matchi.Application.Features.LoanRequests.Commands.CreateLoanRequest;

public sealed class CreateLoanRequestCommandHandler
    : IRequestHandler<CreateLoanRequestCommand, long>
{
    public Task<long> Handle(CreateLoanRequestCommand request, CancellationToken cancellationToken)
    {
        throw new InvalidOperationException(
            "Loan requests are not part of the Matchi baseline model. Use service requests instead.");
    }
}
