namespace Nameless.Gamebuster.Validation;

/// <summary>
/// Provides means to validate an instance of an object.
/// </summary>
public interface IValidationService {
    /// <summary>
    /// Validates <paramref name="value"/> against the rules defined.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value instance.</param>
    /// <param name="throwOnFailure">Indicates whether it should throw an exception on failure.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The validation result.</returns>
    Task<ValidationResult> ValidateAsync<T>(T value, bool throwOnFailure, CancellationToken cancellationToken);
}
