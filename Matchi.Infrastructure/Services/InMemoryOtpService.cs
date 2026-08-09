using System.Collections.Concurrent;
using Matchi.Application.Common.Interfaces;

namespace Matchi.Infrastructure.Services;

public sealed class InMemoryOtpService : IOtpService
{
    private static readonly ConcurrentDictionary<string, OtpEntry> _store = new();
    private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(5);
    private const string DefaultOtpCode = "123456";

    public Task<string> CreateOtpRequestAsync(string mobile, CancellationToken cancellationToken = default)
    {
        var requestId = Guid.NewGuid().ToString("N");
        var entry = new OtpEntry(mobile.Trim(), DefaultOtpCode, DateTime.UtcNow.Add(DefaultTtl));
        _store[requestId] = entry;
        return Task.FromResult(requestId);
    }

    public Task<bool> ValidateOtpAsync(
        string mobile,
        string requestId,
        string otp,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(requestId))
            return Task.FromResult(false);

        if (!_store.TryGetValue(requestId, out var entry))
            return Task.FromResult(false);

        if (!string.Equals(entry.Mobile, mobile.Trim(), StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(entry.Otp, otp.Trim(), StringComparison.Ordinal) ||
            entry.ExpiresAt < DateTime.UtcNow)
        {
            return Task.FromResult(false);
        }

        _store.TryRemove(requestId, out _);
        return Task.FromResult(true);
    }

    private sealed record OtpEntry(string Mobile, string Otp, DateTime ExpiresAt);
}
