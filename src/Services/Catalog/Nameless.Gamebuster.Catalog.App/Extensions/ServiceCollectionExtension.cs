using Nameless.RawgClient.Options;

namespace Nameless.Gamebuster.Catalog.App.Extensions;

internal static class ServiceCollectionExtension {
    internal static IServiceCollection RegisterRawgOptions(this IServiceCollection self, IConfiguration configuration)
        => self.Configure<RawgOptions>(configuration.GetSection(nameof(RawgOptions)));
}
