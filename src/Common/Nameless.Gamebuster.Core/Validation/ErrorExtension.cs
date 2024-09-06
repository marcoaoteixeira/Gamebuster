namespace Nameless.Gamebuster.Validation;

/// <summary>
/// <see cref="Error"/> extension methods.
/// </summary>
public static class ErrorExtension {
    /// <summary>
    /// Transforms a collection of <see cref="Error"/> into a <see cref="Dictionary{TKey,TValue}"/>
    /// </summary>
    /// <param name="self">The collection of <see cref="Error"/> instance.</param>
    /// <returns>A dictionary representing the collection of <see cref="Error"/>.</returns>
    public static Dictionary<string, string[]> ToProblems(this IEnumerable<Error> self)
        => self.ToDictionary(
            keySelector: key => key.Code,
            elementSelector: value => new[] { value.Description }
        );
}
