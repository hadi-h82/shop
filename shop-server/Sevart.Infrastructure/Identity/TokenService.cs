using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Sevart.Application.Abstractions.Identity;

namespace Sevart.Infrastructure.Identity;

public sealed class TokenService : ITokenService
{
    private readonly JwtOptions _options;

    public TokenService(
        IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public GeneratedAuthTokens GenerateTokens(
        Guid userId,
        string email,
        string fullName,
        IReadOnlyCollection<string> roles)
    {
        var now = DateTime.UtcNow;

        var accessTokenExpiresAt =
            now.AddMinutes(
                _options.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new(
                "sub",
                userId.ToString()),

            new(
                "email",
                email),

            new(
                "name",
                fullName),

            new(
                "jti",
                Guid.NewGuid().ToString())
        };

        claims.AddRange(
            roles.Select(
                role => new Claim(
                    "role",
                    role)));

        var signingKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _options.SecretKey));

        var tokenDescriptor =
            new SecurityTokenDescriptor
            {
                Issuer = _options.Issuer,
                Audience = _options.Audience,

                Subject =
                    new ClaimsIdentity(claims),

                IssuedAt = now,
                NotBefore = now,
                Expires = accessTokenExpiresAt,

                SigningCredentials =
                    new SigningCredentials(
                        signingKey,
                        SecurityAlgorithms.HmacSha256)
            };

        var accessToken =
            new JsonWebTokenHandler()
                .CreateToken(tokenDescriptor);

        var refreshToken =
            Base64UrlEncoder.Encode(
                RandomNumberGenerator.GetBytes(64));

        var refreshTokenHash =
            Convert.ToHexString(
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(
                        refreshToken)));

        var refreshTokenExpiresAt =
            now.AddDays(
                _options.RefreshTokenExpirationDays);

        return new GeneratedAuthTokens(
            AccessToken: accessToken,
            AccessTokenExpiresAt: accessTokenExpiresAt,
            RefreshToken: refreshToken,
            RefreshTokenHash: refreshTokenHash,
            RefreshTokenExpiresAt: refreshTokenExpiresAt);
    }
}