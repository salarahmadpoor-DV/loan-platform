using System;
using System.Collections.Generic;

namespace Matchi.Application.Common.Interfaces;

public interface IJwtTokenService
{
JwtTokenResult GenerateToken(
    long userId,
    string mobile,
    IEnumerable<string>? roles = null,
    IEnumerable<string>? permissions = null);
}

public sealed record JwtTokenResult(string AccessToken, DateTime ExpiresAtUtc);

