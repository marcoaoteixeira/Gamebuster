using Microsoft.IdentityModel.Tokens;

namespace Nameless.Gamebuster.Jwt;

public static class JwtOptionsExtension {
    public static TokenValidationParameters GetTokenValidationParameters(this JwtOptions self)
        => new() {
            ValidateIssuer = self.ValidateIssuer,
            ValidateAudience = self.ValidateAudience,
            ValidateLifetime = self.ValidateLifetime,
            ValidateIssuerSigningKey = self.ValidateIssuerSigningKey,
            ValidIssuer = self.Issuer,
            ValidAudience = self.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(self.Secret.GetBytes()),
            ClockSkew = TimeSpan.FromSeconds(self.MaxClockSkew)
        };
}
