namespace Matchi.Application.Features.Banks.Queries.GetBankBySlug;

public sealed class BankDetailDto
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public List<BankQuestionDto> Questions { get; set; }
        = new();
}


public sealed class BankQuestionDto
{
    public long Id { get; set; }

    public string QuestionText { get; set; } = null!;

    public bool IsRequired { get; set; }

    public int SortOrder { get; set; }
}
