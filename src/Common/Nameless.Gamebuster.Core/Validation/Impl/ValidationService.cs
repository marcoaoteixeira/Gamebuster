using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Gamebuster.Validation.Impl;

/// <summary>
/// Current implementation of <see cref="IValidationService"/>.
/// </summary>
/// <param name="provider">The current service provider instance.</param>
public sealed class ValidationService(IServiceProvider provider) : IValidationService {
    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync<T>(T value, bool throwOnFailure, CancellationToken cancellationToken) {
        var validator = provider.GetService<IValidator<T>>();

        if (validator is null) {
            return ValidationResult.Inconclusive($"Missing validator for {typeof(T)}");
        }

        var fluentValidationResult = await validator.ValidateAsync(
            value,
            cancellationToken);

        var result = Transform(fluentValidationResult);

        if (throwOnFailure && !result.Succeeded) {
            throw new ValidationException(result.Errors);
        }

        return result;
    }

    private static ValidationResult Transform(FluentValidation.Results.ValidationResult fluentValidationResult) {
        if (fluentValidationResult.IsValid) {
            return ValidationResult.Success();
        }

        var errors = fluentValidationResult.Errors
                                           .Select(error => new Error {
                                               Code = error.PropertyName,
                                               Description = error.ErrorMessage
                                           })
                                           .ToArray();

        return ValidationResult.Failure(errors);
    }
}
