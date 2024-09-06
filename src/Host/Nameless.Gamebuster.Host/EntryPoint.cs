namespace Nameless.Gamebuster.Host;

internal class EntryPoint {
    private static void Main(string[] args) {
        var builder = DistributedApplication.CreateBuilder(args);

        var catalog = builder
            .AddProject<Projects.Nameless_Gamebuster_Catalog_App>("catalog");

        builder
            .AddProject<Projects.Nameless_Gamebuster_Storefront>("storefront")
            .WithReference(catalog);

        builder.Build()
               .Run();
    }
}