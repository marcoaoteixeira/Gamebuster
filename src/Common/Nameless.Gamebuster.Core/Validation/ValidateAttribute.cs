using System.Reflection;

namespace Nameless.Gamebuster.Validation;

/// <summary>
/// Attribute to identify a type that needs pass through the validation service.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ValidateAttribute : Attribute {
    /// <summary>
    /// Checks if the <see cref="ValidateAttribute"/> is present in the <see cref="object"/>
    /// instance.
    /// </summary>
    /// <param name="obj">The object instance.</param>
    /// <returns><c>true</c> if <see cref="ValidateAttribute"/> is present; otherwise <c>false</c>.</returns>
    public static bool Present(object? obj)
        => obj?.GetType()
              .GetCustomAttribute<ValidateAttribute>(inherit: false) is not null;
}
