using System.Reflection;

namespace Nameless.Gamebuster;

/// <summary>
/// <see cref="Assembly"/> extension methods.
/// </summary>
public static class AssemblyExtension {
    /// <summary>
    /// Searches for all implementations of a given service <typeparamref name="TService"/>
    /// in the current assembly.
    /// <br /><br />
    /// <strong>Note:</strong> we look only for public exported types, never private, protected or internal.
    /// </summary>
    /// <typeparam name="TService">The type of the service, usually the base class or interface.</typeparam>
    /// <param name="self">The current assembly.</param>
    /// <returns>A collection of types that implements <typeparamref name="TService"/>.</returns>
    public static IEnumerable<Type> SearchForImplementations<TService>(this Assembly self)
        => self.GetExportedTypes()
               .Where(type => type is { IsInterface: false, IsAbstract: false } &&
                              typeof(TService).IsAssignableFrom(type));
}
