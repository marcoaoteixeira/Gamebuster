using MediatR;
using Microsoft.Extensions.Options;
using Nameless.Gamebuster.Catalog.App.Infrastructure;
using Nameless.Gamebuster.Catalog.App.Queries;
using Nameless.Gamebuster.Catalog.Objects.Requests;
using Nameless.Gamebuster.Catalog.Objects.Responses;
using Nameless.Gamebuster.Infrastructure;

namespace Nameless.Gamebuster.Catalog.App.Endpoints;

public sealed class SearchGames : IMinimalEndpoint {
    private readonly CatalogOptions _options;

    public string Name => "Search Games";

    public string Summary => "Search Gamebuster games catalog (using RAWG API)";

    public string Description => string.Empty;

    public string Group => "Search";

    public int Version => 1;

    public SearchGames(IOptions<CatalogOptions> options) {
        _options = options.Value;
    }

    public IEndpointConventionBuilder Map(IEndpointRouteBuilder builder)
        => builder
           .MapGet($"{_options.BaseApiPath}/search", HandleAsync)
           .Produces(StatusCodes.Status200OK, typeof(SearchGamesResponse))
           .ProducesValidationProblem()
           .ProducesProblem(StatusCodes.Status500InternalServerError);

    public static async Task<IResult> HandleAsync([AsParameters] SearchGamesRequest request, IMediator mediator, CancellationToken cancellationToken) {
        var response = await mediator.Send(new SearchGamesQuery(request), cancellationToken);

        return response.Succeeded
            ? Results.Ok(response)
            : Results.Problem(response.Error);
    }
}
