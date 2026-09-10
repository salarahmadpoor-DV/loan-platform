using Matchi.Application.Common.Models;

namespace Matchi.Application.Common.Interfaces;

public interface IOtpService
{
    Task<OtpIssueResult> CreateOtpRequestAsync(string mobile, CancellationToken cancellationToken = default);

    Task<bool> ValidateOtpAsync(
        string mobile,
        string requestId,
        string otp,
        CancellationToken cancellationToken = default);
}
