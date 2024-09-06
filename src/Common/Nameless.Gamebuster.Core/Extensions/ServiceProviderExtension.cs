using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Nameless.Gamebuster;

public static class ServiceProviderExtension {
    /// <summary>
    /// Tries to retrieve a service from the current <see cref="IServiceProvider"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <param name="self">The current <see cref="IServiceProvider"/>.</param>
    /// <param name="service">The service, if found.</param>
    /// <returns>
    ///<c>true</c> if it found the service. Otherwise; returns <c>false</c>.
    /// </returns>
    public static bool TryGetService<TService>(this IServiceProvider self, [NotNullWhen(returnValue: true)] out TService? service) {
        service = self.GetService<TService>();

        return service is not null;
    }

    /// <summary>
    /// Retrieves an <see cref="ILogger{TCategoryName}"/> for the <typeparamref name="TCategoryName"/>.
    /// </summary>
    /// <typeparam name="TCategoryName">The type of the category (service).</typeparam>
    /// <param name="self">The current <see cref="IServiceProvider"/></param>
    /// <returns>
    /// An <see cref="ILogger{TCategoryName}"/> if <see cref="ILoggerFactory"/> is available.
    /// Otherwise; a <see cref="NullLogger{T}"/> instance.
    /// </returns>
    public static ILogger<TCategoryName> GetLogger<TCategoryName>(this IServiceProvider self) {
        var loggerFactory = self.GetService<ILoggerFactory>();

        return loggerFactory is not null
            ? loggerFactory.CreateLogger<TCategoryName>()
            : NullLogger<TCategoryName>.Instance;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    /// <param name="self"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    public static IOptions<TOptions> GetOptions<TOptions>(this IServiceProvider self, Func<TOptions>? factory = null)
        where TOptions : class {
        // let's first check if it was already configured. 
        var options = self.GetService<IOptions<TOptions>>();
        if (options is not null) {
            return options;
        }

        // shoot, no good. let's try get from configuration
        if (self.TryGetService<IConfiguration>(out var configuration)) {
            // we need to assume that the section name is the type's name.
            var sectionName = typeof(TOptions).Name;
            var result = configuration.GetSection(sectionName)
                                      .Get<TOptions>();

            if (result is not null) {
                return Options.Create(result);
            }
        }

        // whoops...if we reach this far, seems like we don't have
        // the configuration set or missing this particular option.
        // If we have the factory let's construct it. Otherwise,
        // exception it is.
        ArgumentNullException.ThrowIfNull(factory, nameof(factory));

        return Options.Create(factory());
    }
}
