using System.ComponentModel;
using System.Reflection;

namespace Nameless.Gamebuster;

/// <summary>
/// <see cref="PropertyInfo"/> extension methods
/// </summary>
public static class PropertyInfoExtension {
    /// <summary>
    /// Retrieves the <see cref="DescriptionAttribute.Description"/> value
    /// from the current <see cref="PropertyInfo"/>, if present.
    /// Otherwise; returns the <see cref="PropertyInfo.Name"/>.
    /// </summary>
    /// <param name="self">The property instance.</param>
    /// <returns>The property description attribute value or its name.</returns>
    public static string GetDescription(this PropertyInfo self)
        => GetDescription(self, self.Name);

    /// <summary>
    /// Retrieves the <see cref="DescriptionAttribute.Description"/> value
    /// from the current <see cref="PropertyInfo"/> if present.
    /// Otherwise; the <paramref name="fallback"/>.
    /// </summary>
    /// <param name="self">The property instance.</param>
    /// <param name="fallback">
    /// The fallback value if <see cref="DescriptionAttribute"/> is not present.
    /// </param>
    /// <returns>The property description attribute value or the fallback.</returns>
    public static string GetDescription(this PropertyInfo self, string fallback) {
        var attr = self.GetCustomAttribute<DescriptionAttribute>();

        return attr is not null ? attr.Description : fallback;
    }
}
