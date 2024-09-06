namespace Nameless.Gamebuster.Validation;

/// <summary>
/// <see cref="ValidationResult"/> extension methods.
/// </summary>
public static class ValidationResultExtension {
    /// <summary>
    /// Transforms an instance of <see cref="ValidationResult"/> into a <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="self">The <see cref="ValidationResult"/> instance.</param>
    /// <returns>A dictionary representing the instance of <see cref="ValidationResult"/>.</returns>
    public static Dictionary<string, string[]> ToProblems(this ValidationResult self) {
        // if not successful, let's create the problems dictionary.
        if (!self.Succeeded) {
            return self.Errors.ToProblems();
        }

        // if success but there is a reason, creates the dictionary
        // with the reason.
        if (!string.IsNullOrWhiteSpace(self.Reason)) {
            return new Dictionary<string, string[]> {
                { nameof(self.Reason), [self.Reason] }
            };
        }

        // everything cool? so empty dictionary.
        return new Dictionary<string, string[]>();
    }
}
