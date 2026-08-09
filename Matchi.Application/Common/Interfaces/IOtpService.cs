using System.Threading;
using System.Threading.Tasks;

namespace Matchi.Application.Common.Interfaces;

public interface IOtpService
{
    Task<string> CreateOtpRequestAsync(string mobile, CancellationToken cancellationToken = default);

    Task<bool> ValidateOtpAsync(
        string mobile,
        string requestId,
        string otp,
        CancellationToken cancellationToken = default);
}
