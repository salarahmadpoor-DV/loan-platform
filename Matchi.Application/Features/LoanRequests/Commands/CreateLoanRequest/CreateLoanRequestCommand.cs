using MediatR;

namespace Matchi.Application.Features.LoanRequests.Commands.CreateLoanRequest;

public sealed record CreateLoanRequestCommand : IRequest<long>
{
    public long BankId { get; init; }

    public long UserId { get; init; }
}