using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Common.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Matchi.Infrastructure.Services;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public JwtTokenResult GenerateToken(
        long userId,
        string mobile,
        IEnumerable<string>? roles = null,
        IEnumerable<string>? permissions = null)
    {
        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(_jwtSettings.ExpiryMinutes);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (!string.IsNullOrWhiteSpace(mobile))
        {
            claims.Add(
                new Claim(ClaimTypes.MobilePhone, mobile));
        }

        // Roles
        if (roles is not null)
        {
            claims.AddRange(
                roles
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => new Claim(
                        ClaimTypes.Role,
                        x.Trim())));
        }

        // Permissions
        if (permissions is not null)
        {
            claims.AddRange(
                permissions
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => new Claim(
                        "permission",
                        x.Trim())));
        }

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: credentials);

        var token =
            new JwtSecurityTokenHandler().WriteToken(jwt);

        return new JwtTokenResult(token, expiresAt);
    }
}