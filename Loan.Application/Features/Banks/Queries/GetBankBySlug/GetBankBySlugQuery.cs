using MediatR;

namespace Loan.Application.Features.Banks.Queries.GetBankBySlug;

public sealed record GetBankBySlugQuery(string Slug)
    : IRequest<BankDetailDto?>;