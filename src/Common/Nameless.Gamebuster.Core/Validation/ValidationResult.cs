namespace Nameless.Gamebuster.Validation;

/// <summary>
/// Provides properties and methods to present a validation result.
/// </summary>
public sealed record ValidationResult {
    /// <summary>
    /// Gets the errors of this result.
    /// </summary>
    public Error[] Errors { get; }

    /// <summary>
    /// Gets the reason for this validation result.
    /// </summary>
    public string Reason { get; }

    /// <summary>
    /// Whether the validation succeeded or not.
    /// </summary>
    public bool Succeeded => Errors.Length == 0;

    private ValidationResult(Error[] errors, string reason) {
        Errors = errors;
        Reason = reason;
    }

    /// <summary>
    /// Creates a success validation result.
    /// </summary>
    /// <returns>An instance of <see cref="ValidationResult"/>.</returns>
    public static ValidationResult Success()
        => new(errors: [], reason: string.Empty);

    /// <summary>
    /// Creates a failed validation result.
    /// </summary>
    /// <returns>An instance of <see cref="ValidationResult"/>.</returns>
    public static ValidationResult Failure(IEnumerable<Error> errors)
        => new(errors: errors.ToArray(), reason: string.Empty);

    /// <summary>
    /// Creates an inconclusive validation result.
    /// </summary>
    /// <param name="reason">The reason why was inconclusive. Must be provided.</param>
    /// <returns>An instance of <see cref="ValidationResult"/>.</returns>
    public static ValidationResult Inconclusive(string reason) {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        return new ValidationResult(errors: [], reason);
    }
}
