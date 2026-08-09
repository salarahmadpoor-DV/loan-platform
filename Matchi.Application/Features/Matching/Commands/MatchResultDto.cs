namespace Matchi.Application.Features.Matching.Commands;

public sealed class MatchResultDto
{
    public IEnumerable<MatchCandidateDto> Providers { get; init; } = Array.Empty<MatchCandidateDto>();
    public IEnumerable<MatchCandidateDto> Businesses { get; init; } = Array.Empty<MatchCandidateDto>();

    public MatchResultDto(
        IEnumerable<MatchCandidateDto> providers,
        IEnumerable<MatchCandidateDto> businesses)
    {
        Providers = providers;
        Businesses = businesses;
    }
}

public sealed class MatchCandidateDto
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public string Type { get; init; } = null!;

    public MatchCandidateDto(long id, string name, string type)
    {
        Id = id;
        Name = name;
        Type = type;
    }
}
