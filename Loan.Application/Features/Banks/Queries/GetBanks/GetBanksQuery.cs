using MediatR;
namespace Loan.Application.Features.Banks.Queries.GetBanks;

public sealed record GetBanksQuery 
    : IRequest<List<BankDto>>;