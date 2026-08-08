using MediatR;

namespace Matchi.Application.Features.Banks.Queries.GetBankBySlug;

public sealed record GetBankBySlugQuery(string Slug)
    : IRequest<BankDetailDto?>;