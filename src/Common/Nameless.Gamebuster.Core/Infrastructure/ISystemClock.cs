namespace Nameless.Gamebuster.Infrastructure;

/// <summary>
/// Provides means to retrieve system <see cref="DateTime"/> and <see cref="DateTimeOffset"/>.
/// </summary>
public interface ISystemClock {
    /// <summary>
    /// Retrieves system current <see cref="DateTime.UtcNow"/>.
    /// </summary>
    /// <returns>System current <see cref="DateTime.UtcNow"/></returns>
    DateTime GetUtcNow();

    /// <summary>
    /// Retrieves system current <see cref="DateTimeOffset.UtcNow"/>.
    /// </summary>
    /// <returns>System current <see cref="DateTimeOffset.UtcNow"/></returns>
    DateTimeOffset GetUtcNowOffset();
}
