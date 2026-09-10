using Matchi.Application.Common.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Auth.Commands.SendOtp;

public sealed class SendOtpCommandHandler : IRequestHandler<SendOtpCommand, string>
{
    private readonly IOtpService _otpService;

    public SendOtpCommandHandler(IOtpService otpService)
    {
        _otpService = otpService;
    }

    public async Task<string> Handle(
        SendOtpCommand request,
        CancellationToken cancellationToken)
    {
        var issued = await _otpService.CreateOtpRequestAsync(request.Mobile.Trim(), cancellationToken);
        return issued.RequestId;
    }
}
