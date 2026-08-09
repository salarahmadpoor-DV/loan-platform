using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Matchi.Application.Features.Auth.Commands.SendOtp;
using Matchi.Application.Features.Auth.Commands.VerifyOtp;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public record SendOtpRequest(string Mobile);
        public record VerifyOtpRequest(string Mobile, string Otp, string RequestId);

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp(
            [FromBody] SendOtpRequest request,
            CancellationToken cancellationToken = default)
        {
            var requestId = await _mediator.Send(
                new SendOtpCommand(request.Mobile),
                cancellationToken);

            return Accepted(new { requestId });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(
            [FromBody] VerifyOtpRequest request,
            CancellationToken cancellationToken = default)
        {
            var authResult = await _mediator.Send(
                new VerifyOtpCommand(request.Mobile, request.Otp, request.RequestId),
                cancellationToken);

            return Ok(authResult);
        }
    }
}
