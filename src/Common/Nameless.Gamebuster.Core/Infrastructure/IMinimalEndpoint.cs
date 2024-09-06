using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Nameless.Gamebuster.Infrastructure;

/// <summary>
/// Defines properties and methods to implement a minimal API service.
/// </summary>
public interface IMinimalEndpoint {
    string Name { get; }
    string Summary { get; }
    string Description { get; }
    string Group { get; }
    int Version { get; }

    IEndpointConventionBuilder Map(IEndpointRouteBuilder builder);
}
