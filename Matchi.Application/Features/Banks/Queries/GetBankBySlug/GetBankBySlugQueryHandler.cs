using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Banks.Queries.GetBankBySlug;

public sealed class GetBankBySlugQueryHandler
    : IRequestHandler<GetBankBySlugQuery, BankDetailDto?>
{
    private readonly IBankRepository _repository;

    public GetBankBySlugQueryHandler(
        IBankRepository repository)
    {
        _repository = repository;
    }

    public async Task<BankDetailDto?> Handle(
        GetBankBySlugQuery request,
        CancellationToken cancellationToken)
    {
        var bank = await _repository.GetBySlugAsync(
            request.Slug,
            cancellationToken);

        if (bank == null)
            return null;

        return new BankDetailDto
        {
            Id = bank.Id,
            Title = bank.Title,
            Slug = bank.Slug,
            Description = bank.Description,

            Questions = bank.Questions
                .Where(q => !q.IsDeleted)
                .OrderBy(q => q.SortOrder)
                .Select(q => new BankQuestionDto
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    IsRequired = q.IsRequired,
                    SortOrder = q.SortOrder
                })
                .ToList()
        };
    }
}