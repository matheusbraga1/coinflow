using CoinFlow.Application.Abstractions.Authentication;
using CoinFlow.Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace CoinFlow.Infrastructure.Authentication;

internal sealed class JwtTokenGenerator : ITokenGenerator
{
    private readonly JwtSettings _settings;
    private static readonly JsonWebTokenHandler _handler = new();

    public JwtTokenGenerator(IOptions<JwtSettings> options) => _settings = options.Value;

    public TokenPair Generate(User user)
    {
        var now = DateTime.UtcNow;
        var accessExpiresAt = now.AddMinutes(_settings.AccessTokenMinutes);
        var refreshExpiresAt = now.AddDays(_settings.RefreshTokenDays);

        var accessToken = GenerateAccessToken(user, accessExpiresAt, now);
        var refreshToken = GenerateRefreshToken();

        return new TokenPair(accessToken, refreshToken, accessExpiresAt, refreshExpiresAt);
    }

    private string GenerateAccessToken(User user, DateTime expiresAt, DateTime now)
    {
        var keyBytes = Encoding.UTF8.GetBytes(_settings.SecretKey);
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            NotBefore = now,
            Expires = expiresAt,
            SigningCredentials = signingCredentials,
            Claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Sub] = user.Id.ToString(),
                [JwtRegisteredClaimNames.Email] = user.Email.Value,
                [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
                ["name"] = user.Name
            }
        };

        return _handler.CreateToken(descriptor);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}
