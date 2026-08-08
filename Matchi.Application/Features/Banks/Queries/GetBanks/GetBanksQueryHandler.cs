using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Banks.Queries.GetBanks;

public sealed class GetBanksQueryHandler 
    : IRequestHandler<GetBanksQuery, List<BankDto>>
{
    private readonly IBankRepository _bankRepository;

    public GetBanksQueryHandler(
        IBankRepository bankRepository)
    {
        _bankRepository = bankRepository;
    }

    public async Task<List<BankDto>> Handle(
        GetBanksQuery request,
        CancellationToken cancellationToken)
    {
        var banks = await _bankRepository
            .GetAllAsync(cancellationToken);

        return banks.Select(x => new BankDto
        {
            Id = x.Id,
            Title = x.Title,
            Slug = x.Slug,

            ShortDescription = x.ShortDescription,
            Description = x.Description,

            TelegramLink = x.TelegramLink,
            EitaaLink = x.EitaaLink,
            RubikaLink = x.RubikaLink,

            SeoTitle = x.SeoTitle,
            SeoDescription = x.SeoDescription,

            SortOrder = x.SortOrder,
            IsActive = x.IsActive

        }).ToList();
    }
}