using System.Text;

namespace Nameless.Gamebuster;

/// <summary>
/// <see cref="string"/> extension methods.
/// </summary>
public static class StringExtension {
    /// <summary>
    /// Retrieves the bytes for a string instance. If <paramref name="encoding"/>
    /// is not provided, it will use the default encoding (UTF-8 without BOM).
    /// </summary>
    /// <param name="self">The string instance.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>An array of bytes representing the string instance.</returns>
    public static byte[] GetBytes(this string self, Encoding? encoding = null) {
        return (encoding ?? Root.Defaults.Encoding).GetBytes(self);
    }
}
