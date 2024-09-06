using System.ComponentModel;
using System.Reflection;

namespace Nameless.Gamebuster;

/// <summary>
/// <see cref="Enum"/> extension methods.
/// </summary>
public static class EnumExtension {
    /// <summary>
    /// Retrieves the <see cref="DescriptionAttribute.Description"/> value
    /// from the current <see cref="Enum"/>, if present.
    /// Otherwise; returns the <see cref="Enum"/> string representation.
    /// </summary>
    /// <param name="self">The enum instance.</param>
    /// <returns>The enum description attribute value or its string representation.</returns>
    public static string GetDescription(this Enum self)
        => GetDescription(self, self.ToString());


    /// <summary>
    /// Retrieves the <see cref="DescriptionAttribute.Description"/> value
    /// from the current <see cref="Enum"/> if present.
    /// Otherwise; the <paramref name="fallback"/>.
    /// </summary>
    /// <param name="self">The enum instance.</param>
    /// <param name="fallback">
    /// The fallback value if <see cref="DescriptionAttribute"/> is not present.
    /// </param>
    /// <returns>The enum description attribute value or the fallback.</returns>
    public static string GetDescription(this Enum self, string fallback) {
        var field = self.GetType()
                        .GetField(self.ToString());

        if (field is null) { return fallback; }

        var attr = field.GetCustomAttribute<DescriptionAttribute>(inherit: false);

        return attr is not null ? attr.Description : fallback;
    }
}
