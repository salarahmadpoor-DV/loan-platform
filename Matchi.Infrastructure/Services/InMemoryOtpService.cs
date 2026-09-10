using System.Collections.Concurrent;
using System.Security.Cryptography;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Common.Models;

namespace Matchi.Infrastructure.Services;

public sealed class InMemoryOtpService : IOtpService
{
    private readonly ConcurrentDictionary<string, OtpEntry> _store = new();
    private readonly TimeProvider _timeProvider;
    private readonly OtpOptions _options;

    public InMemoryOtpService(TimeProvider timeProvider, OtpOptions options)
    {
        _timeProvider = timeProvider;
        _options = options;
    }

    public InMemoryOtpService()
        : this(TimeProvider.System, new OtpOptions())
    {
    }

    public Task<OtpIssueResult> CreateOtpRequestAsync(string mobile, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var ttl = TimeSpan.FromMinutes(Math.Max(1, _options.TtlMinutes));
        var requestId = Guid.NewGuid().ToString("N");
        var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        var entry = new OtpEntry(
            mobile.Trim(),
            code,
            _timeProvider.GetUtcNow().UtcDateTime.Add(ttl),
            FailedAttempts: 0);

        _store[requestId] = entry;
        return Task.FromResult(new OtpIssueResult(requestId, code));
    }

    public Task<bool> ValidateOtpAsync(
        string mobile,
        string requestId,
        string otp,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(requestId) || string.IsNullOrWhiteSpace(otp))
            return Task.FromResult(false);

        if (!_store.TryGetValue(requestId, out var entry))
            return Task.FromResult(false);

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        if (entry.ExpiresAt < now)
        {
            _store.TryRemove(requestId, out _);
            return Task.FromResult(false);
        }

        var mobileMatches = string.Equals(entry.Mobile, mobile.Trim(), StringComparison.OrdinalIgnoreCase);
        var otpMatches = string.Equals(entry.Code, otp.Trim(), StringComparison.Ordinal);

        if (mobileMatches && otpMatches)
        {
            _store.TryRemove(requestId, out _);
            return Task.FromResult(true);
        }

        var attempts = entry.FailedAttempts + 1;
        var maxAttempts = Math.Max(1, _options.MaxAttempts);
        if (attempts >= maxAttempts)
        {
            _store.TryRemove(requestId, out _);
            return Task.FromResult(false);
        }

        _store[requestId] = entry with { FailedAttempts = attempts };
        return Task.FromResult(false);
    }

    private sealed record OtpEntry(string Mobile, string Code, DateTime ExpiresAt, int FailedAttempts);
}
