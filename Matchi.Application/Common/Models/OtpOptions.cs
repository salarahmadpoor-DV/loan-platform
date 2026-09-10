namespace Matchi.Application.Common.Models;

public sealed class OtpOptions
{
    public const string SectionName = "Otp";

    public int TtlMinutes { get; set; } = 5;

    public int MaxAttempts { get; set; } = 5;
}

public sealed record OtpIssueResult(string RequestId, string Code);
