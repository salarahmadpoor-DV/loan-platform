using Loan.Domain.Interfaces;
using MediatR;

namespace Loan.Application.Features.LoanRequests.Commands.CreateLoanRequest;

public sealed class CreateLoanRequestCommandHandler
    : IRequestHandler<CreateLoanRequestCommand, long>
{
    private readonly ILoanRequestRepository _loanRequestRepository;

    public CreateLoanRequestCommandHandler(
        ILoanRequestRepository loanRequestRepository)
    {
        _loanRequestRepository = loanRequestRepository;
    }

    public async Task<long> Handle(
        CreateLoanRequestCommand request,
        CancellationToken cancellationToken)
    {
        return await _loanRequestRepository.CreateAsync(
            request.BankId,
            request.UserId,
            cancellationToken);
    }
}