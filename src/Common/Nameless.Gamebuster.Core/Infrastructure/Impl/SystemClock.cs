namespace Nameless.Gamebuster.Infrastructure.Impl;

/// <summary>
/// Implementation of <see cref="ISystemClock"/>.
/// </summary>
public sealed class SystemClock : ISystemClock {
    /// <summary>
    /// Gets the unique instance of <see cref="SystemClock" />.
    /// </summary>
    public static ISystemClock Instance { get; } = new SystemClock();

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static SystemClock() { }

    private SystemClock() { }

    /// <inheritdoc />
    public DateTime GetUtcNow()
        => DateTime.UtcNow;

    /// <inheritdoc />
    public DateTimeOffset GetUtcNowOffset()
        => DateTimeOffset.UtcNow;
}
