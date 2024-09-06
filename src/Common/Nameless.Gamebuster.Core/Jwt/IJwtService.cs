using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Nameless.Gamebuster.Jwt;

public interface IJwtService {
    #region Methods

    string Generate(JwtClaims claims);

    bool TryValidate(string token, [NotNullWhen(returnValue: true)] out ClaimsPrincipal? principal);

    #endregion
}
