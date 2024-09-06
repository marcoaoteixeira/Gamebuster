using MediatR;
using Nameless.Gamebuster.Catalog.Objects.Requests;
using Nameless.Gamebuster.Catalog.Objects.Responses;

namespace Nameless.Gamebuster.Storefront.Core.Queries.Catalog;

public sealed record SearchGamesQuery : IRequest<SearchGamesResponse> {
    public SearchGamesRequest Parameters { get; }

    public SearchGamesQuery(SearchGamesRequest parameters) {
        ArgumentNullException.ThrowIfNull(parameters, nameof(parameters));

        Parameters = parameters;
    }
}