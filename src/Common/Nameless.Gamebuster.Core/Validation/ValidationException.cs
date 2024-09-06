namespace Nameless.Gamebuster.Validation;

/// <summary>
/// Represents a validation exception.
/// </summary>
public class ValidationException : Exception {
    /// <summary>
    /// Gets the list of errors that generate this exception.
    /// </summary>
    public Error[] Errors { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationException"/>.
    /// </summary>
    public ValidationException()
        : this([]) { }

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationException"/>.
    /// </summary>
    /// <param name="errors">The list of errors.</param>
    public ValidationException(Error[] errors)
        : base(string.Empty, null) {
        Errors = errors;
    }
}
