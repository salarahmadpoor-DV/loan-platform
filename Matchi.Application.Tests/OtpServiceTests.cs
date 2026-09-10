using Matchi.Application.Common.Models;
using Matchi.Infrastructure.Services;

namespace Matchi.Application.Tests;

public sealed class OtpServiceTests
{
    [Fact]
    public async Task Generated_otp_can_be_verified()
    {
        var service = new InMemoryOtpService();
        var issued = await service.CreateOtpRequestAsync("09120000000");

        var ok = await service.ValidateOtpAsync("09120000000", issued.RequestId, issued.Code);

        Assert.True(ok);
    }

    [Fact]
    public async Task Invalid_otp_fails()
    {
        var service = new InMemoryOtpService();
        var issued = await service.CreateOtpRequestAsync("09120000000");
        var wrong = issued.Code == "000000" ? "000001" : "000000";

        var ok = await service.ValidateOtpAsync("09120000000", issued.RequestId, wrong);

        Assert.False(ok);
    }

    [Fact]
    public async Task Expired_otp_fails()
    {
        var clock = new MutableTimeProvider(DateTimeOffset.UtcNow);
        var service = new InMemoryOtpService(clock, new OtpOptions { TtlMinutes = 5, MaxAttempts = 5 });
        var issued = await service.CreateOtpRequestAsync("09120000000");

        clock.Advance(TimeSpan.FromMinutes(6));

        var ok = await service.ValidateOtpAsync("09120000000", issued.RequestId, issued.Code);

        Assert.False(ok);
    }

    [Fact]
    public async Task Excessive_attempts_fail()
    {
        var service = new InMemoryOtpService(TimeProvider.System, new OtpOptions { TtlMinutes = 5, MaxAttempts = 3 });
        var issued = await service.CreateOtpRequestAsync("09120000000");
        var wrong = issued.Code == "000000" ? "000001" : "000000";

        Assert.False(await service.ValidateOtpAsync("09120000000", issued.RequestId, wrong));
        Assert.False(await service.ValidateOtpAsync("09120000000", issued.RequestId, wrong));
        Assert.False(await service.ValidateOtpAsync("09120000000", issued.RequestId, wrong));

        var afterLock = await service.ValidateOtpAsync("09120000000", issued.RequestId, issued.Code);
        Assert.False(afterLock);
    }

    [Fact]
    public async Task Static_well_known_code_is_not_accepted_unless_it_was_generated()
    {
        var service = new InMemoryOtpService();
        OtpIssueResult issued;
        do
        {
            issued = await service.CreateOtpRequestAsync("09120000000");
        } while (issued.Code == "123456");

        var ok = await service.ValidateOtpAsync("09120000000", issued.RequestId, "123456");

        Assert.False(ok);
    }

    private sealed class MutableTimeProvider : TimeProvider
    {
        private DateTimeOffset _utc;

        public MutableTimeProvider(DateTimeOffset utc) => _utc = utc;

        public void Advance(TimeSpan delta) => _utc += delta;

        public override DateTimeOffset GetUtcNow() => _utc;
    }
}
