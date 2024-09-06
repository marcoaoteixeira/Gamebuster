using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nameless.Gamebuster.Infrastructure;
using Nameless.Gamebuster.Jwt;
using MS_JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Nameless.Gamebuster.Jwt.Impl;

public sealed class JwtService : IJwtService {
    private readonly JwtOptions _options;
    private readonly ISystemClock _clock;
    private readonly ILogger _logger;

    public JwtService(IOptions<JwtOptions> options, ISystemClock clock, ILogger<JwtService> logger) {
        _options = options.Value;
        _clock = clock;
        _logger = logger;
    }

    public string Generate(JwtClaims claims) {
        var now = _clock.GetUtcNow();
        var expires = now.AddHours(_options.AccessTokenTtl);

        var tokenDescriptor = new SecurityTokenDescriptor {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Claims = new Dictionary<string, object> {
                { MS_JwtRegisteredClaimNames.Exp, expires.ToString(CultureInfo.InvariantCulture) },
                { MS_JwtRegisteredClaimNames.Iat, now.ToString(CultureInfo.InvariantCulture) }, {
                    MS_JwtRegisteredClaimNames.Jti, Guid.NewGuid()
                                                        .ToString()
                }
            },
            Expires = expires,
            SigningCredentials = new SigningCredentials(
                key: new SymmetricSecurityKey(_options.Secret.GetBytes()),
                algorithm: SecurityAlgorithms.HmacSha256Signature
            )
        };

        // Add other claims
        var dictionary = claims.ToDictionary();
        foreach (var kvp in dictionary) {
            if (!tokenDescriptor.Claims.TryAdd(kvp.Key, kvp.Value)) {
                _logger.LogInformation(message: "Claim not added to token descriptor. {ClaimType}",
                                       args: kvp.Key);
            }
        }

        if (!string.IsNullOrEmpty(_options.Issuer)) {
            tokenDescriptor.Claims.Add(MS_JwtRegisteredClaimNames.Iss, _options.Issuer);
        }

        if (!string.IsNullOrEmpty(_options.Audience)) {
            tokenDescriptor.Claims.Add(MS_JwtRegisteredClaimNames.Aud, _options.Audience);
        }

        // Force JwtSecurityTokenHandler to use the default claim name
        // https://stackoverflow.com/questions/57998262/why-is-claimtypes-nameidentifier-not-mapping-to-sub
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public bool TryValidate(string token, [NotNullWhen(true)] out ClaimsPrincipal? principal) {
        principal = null;

        try {
            principal = new JwtSecurityTokenHandler()
                .ValidateToken(token: token,
                               validationParameters: _options.GetTokenValidationParameters(),
                               validatedToken: out var securityToken);

            var validate = securityToken is JwtSecurityToken jwtSecurityToken &&
                           jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

            if (!validate) {
                principal = null;
            }

            return validate;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error while validation JWT");
        }

        return false;
    }
}
