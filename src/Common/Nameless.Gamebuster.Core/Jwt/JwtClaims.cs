using System.ComponentModel;
using System.Reflection;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Nameless.Gamebuster.Jwt;

public sealed record JwtClaims {
    /// <summary>
    /// Gets or sets the subject identifier. <see cref="JwtRegisteredClaimNames.Sub"/>.
    /// </summary>
    [Description(JwtRegisteredClaimNames.Sub)]
    public required string Sub { get; init; }

    /// <summary>
    /// Gets or sets the subject name. <see cref="JwtRegisteredClaimNames.Name"/>.
    /// </summary>
    [Description(JwtRegisteredClaimNames.Name)]
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the subject email. <see cref="JwtRegisteredClaimNames.Email"/>.
    /// </summary>
    [Description(JwtRegisteredClaimNames.Email)]
    public required string Email { get; init; }

    /// <summary>
    /// Gets or sets the subject birthdate. <see cref="JwtRegisteredClaimNames.Birthdate"/>.
    /// </summary>
    [Description(JwtRegisteredClaimNames.Birthdate)]
    public string? Birthdate { get; init; }

    /// <summary>
    /// Gets or sets the subject gender. <see cref="JwtRegisteredClaimNames.Gender"/>.
    /// </summary>
    [Description(JwtRegisteredClaimNames.Gender)]
    public string? Gender { get; init; }

    /// <summary>
    /// Gets or sets the subject profile picture.
    /// </summary>
    [Description("picture")]
    public string? Picture { get; init; }

    /// <summary>
    /// Gets or sets the subject locale.
    /// </summary>
    [Description("locale")]
    public string? Locale { get; init; }

    public Dictionary<string, string> ToDictionary()
        => GetType()
           .GetProperties(BindingFlags.Instance | BindingFlags.Public)
           .ToDictionary(
               keySelector: key => key.GetDescription(fallback: key.Name),
               elementSelector: value => value.GetValue(obj: this, index: null)
                                              ?.ToString() ?? string.Empty
           );
}
