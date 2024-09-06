namespace Nameless.Gamebuster.Validation;

/// <summary>
/// Defines an error with code and description.
/// </summary>
public sealed record Error {
    /// <summary>
    /// Gets or init the code for this error.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Gets or init the description for this error.
    /// </summary>
    public required string Description { get; init; }
}
