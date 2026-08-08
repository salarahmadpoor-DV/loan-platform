using MediatR;
namespace Matchi.Application.Features.Banks.Queries.GetBanks;

public sealed record GetBanksQuery 
    : IRequest<List<BankDto>>;